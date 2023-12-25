# EVH file format

Event Horizon recordings are stored in the EVH file format. These are JSON files compressed using Brotli with maximum compression settings, offering a balance of accessibility and efficiency. This document aims to demystify the file format in it's current version.

## Why JSON & Brotli?

JSON (JavaScript Object Notation) was chosen for its adaptability and extensive support in a multitude of applications and programming languages. Its human-readable structure is a key advantage, simplifying the process of understanding the data. This aspect is crucial in research environments, where there is a frequent need to manually examine and integrate data from diverse sources and formats.

In preliminary tests, we have seen that serializing movement data in plain-text JSON can result in very large files. Taking advantage of the repetitiveness of the plain-text data representation of the serialized data, we were able to minimize the file sizes while retaining familiar formats for researchers. We chose Brotli, a widely used codec on the modern web, as it offers better compression as compared to gzip with minimal impact on decompression.

## File Structure
The EVH file format utilizes a JSON-based structure to encapsulate event recording data.
To facilitate understanding and utilization of this format, and to offer automated understanding of the file format,
a JSON schema detailing the structure of the currently adopted recording version is available at
`Documentation~\misc\evh-v1.schema.json` relative to the package root directory.

The following sections offer a detailed breakdown of the format's current (V1) iteration.

### Root Object
- `version` (Number): Indicates the version of the file format. Currently, the only valid value is `0`, corresponding to `RecordingFormatVersion::V1`.
```json
{
  "version": 0,
  "metadata": {...},
  "frames": [...]
}
```

### `RecordingMetadata` Object
This object provides contextual information about the recording:
- `sceneName` (String): The name of the scene being recorded.-
- `fps` (FrameRate Object): The frame rate at which the recording was made, represented as a ratio (numerator/denominator).

```json
"metadata": {
  "sceneName": "ExampleScene",
  "fps": {
    "numerator": 30,
    "denominator": 1
  }
}
```

### `FrameRate` Object
Defines the frame rate as an integer ratio.

- `numerator` (Integer): The numerator of the frame rate ratio.
- `denominator` (Integer): The denominator of the frame rate ratio.


```json
"fps": {
  "numerator": 30,
  "denominator": 1
}
```

### `RecordingFrameData` Array

Each element in this array represents data for a single frame:

- `frame` (Integer): The frame number.
- `timeCode` (Number): The time code of the frame.
- `trackers` (Array): An array of RecordingTrackerData objects, representing tracking data for various objects.

```json
"frames": [
  {
    "frame": 1,
    "timeCode": 0.033,
    "trackers": [...]
  },
  ...
]
```
### `RecordingTrackerData` Object

Contains tracking data for a single object:

- `id` (TrackableID Object): A unique identifier for the tracked object.
- `transform` (TransformData Object): The transform data for the object.

```json
"trackers": [
  {
    "id": {"internal": 123},
    "transform": {...}
  },
  ...
]
```

### `TrackableID` Object

Represents a unique identifier for a trackable object:

- `internal` (Integer): An internal identifier. Required due to the use of the Unity serializer.

```json
"id": {
  "internal": 123
}
```

### TransformData Object

Describes the position, rotation, and scale of an object:

- `position` (Vector3 Object): The position of the object in 3D space.
- `rotation` (Quaternion Object): The rotation of the object.
- `scale` (Vector3 Object): The scale of the object.

```json
"transform": {
  "position": {"x": 1.0, "y": 2.0, "z": 3.0},
  "rotation": {"x": 0.0, "y": 0.0, "z": 0.0, "w": 1.0},
  "scale": {"x": 1.0, "y": 1.0, "z": 1.0}
}
```

### `Vector3` and `Quaternion` Objects

Represent 3D vectors and rotations:
- `Vector3` includes x, y, and z fields (Numbers) representing the coordinates.
- `Quaternion` includes x, y, z, and w fields (Numbers) representing the quaternion rotation.

```json
"position": {"x": 1.0, "y": 2.0, "z": 3.0},
"rotation": {"x": 0.0, "y": 0.7071, "z": 0.0, "w": 0.7071},
```

## Working with the file format
This section provides guidance on how to work with the EVH file format effectively, offering insights into various methods and tools available for interacting with this data format.

### Using the C# API
For those operating within the Unity framework, the EventHorizon assembly provides a straightforward API to interact with the EVH file format. To use this API, include the EventHorizon namespace in your C# script as follows:

```csharp
using EventHorizon;

class Program {
  void Main() {
    // Load the EVH recording using the provided utility function
    RecordingData recording = RecordingDataUtilities.Load("path/to/recording.evh");

    // Proceed with processing the loaded recording data
    // ...
  }
}
```

This approach enables seamless integration and manipulation of recording data within Unity projects. However, usage outside the Unity environment is currently not verified and may not be supported. This is primarily due to the reliance on Unity's JsonUtility API for implementation. While the data classes are standard .NET code, they utilize the SerializeField attribute, which is specific to Unity. For non-Unity contexts, this attribute might require replacement with an alternative or a custom implementation to ensure compatibility.

### Python

Since EVH files encapsulate JSON data compressed with Brotli, the following example code illustrates a straightforward and minimal method to decompress and read EVH files:

```python

import brotli # pip install brotli
import json

with open("path/to/recording_file.evh", "br") as file:
    print(json.loads(brotli.decompress(file.read())))
```

To make integration easier for researchers, we wrote a small python script, `evh.py`, that massages the data to better match the needs of data researchers.
Below is an example for using `evh.py` to get the total distances for each tracker (using pandas):

```python
import evh
import os
import pandas as pd
import numpy as np

def calculate_distance(pos1, pos2):
    return np.sqrt(
        (pos2[0] - pos1[0]) ** 2 + (pos2[1] - pos1[1]) ** 2 + (pos2[2] - pos1[2]) ** 2
    )


def total_distances(recording: evh.Recording) -> pd.Series:
    temp_df = recording.create_tracker_pandas_view().copy()
    temp_df[['X', 'Y', 'Z']] = pd.DataFrame(temp_df['Position'].tolist(), index=temp_df.index)

    # Sort by TrackerID and Frame
    temp_df.sort_values(by=['TrackerID', 'Frame'], inplace=True)

    # Calculate the positional differences
    temp_df[['Next_X', 'Next_Y', 'Next_Z']] = temp_df.groupby('TrackerID')[['X', 'Y', 'Z']].shift(-1)
    temp_df.dropna(subset=['Next_X', 'Next_Y', 'Next_Z'], inplace=True)

    # Calculate distance moved in each frame
    temp_df['Distance'] = temp_df.apply(lambda row: calculate_distance((row['X'], row['Y'], row['Z']), (row['Next_X'], row['Next_Y'], row['Next_Z'])), axis=1)

    # Calculate total distance for each tracker
    total_distances = temp_df.groupby('TrackerID')['Distance'].sum()

    return total_distances


data = evh.Recording.from_file("path/to/recording_file.evh")
avg_speeds = total_distances(data)
print(avg_speeds)
```

### Other languages

Assuming you have a brotli decompressor, you can use the provided JSON schema to generate code for parsing the EVH file format.
