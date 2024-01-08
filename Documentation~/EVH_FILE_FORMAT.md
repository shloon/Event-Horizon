# EVH file format
Event Horizon recordings are stored in the EVH file format, a packet-based file format. These are JSON Lines files compressed using Brotli with maximum compression settings, offering a balance of accessibility and efficiency. This document aims to demystify the file format in it's current version.

## Why JSON & Brotli?
JSON (JavaScript Object Notation) was chosen for its adaptability and extensive support in a multitude of applications and programming languages. Its human-readable structure is a key advantage, simplifying the process of understanding the data. This aspect is crucial in research environments, where there is a frequent need to manually examine and integrate data from diverse sources and formats. To simplify the serialization and the processing of the data, we utilize the json lines format, which stores one json object per line.

In preliminary tests, we have seen that serializing movement data in plain-text JSON can result in very large files. Taking advantage of the repetitiveness of the plain-text data representation of the serialized data and the packet structure, we were able to minimize the file sizes while retaining familiar formats for researchers. We chose Brotli, a widely used codec on the modern web, as it offers better compression as compared to gzip with minimal impact on decompression.

## Overview of the Packet-Based Approach
The packet-based format represents a fundamental change, focusing on structuring the data into discrete packets. Each packet is prefaced with a header that identifies its type, making it straightforward to parse and process. This method significantly enhances the ability to manage multiple objects and diverse data types within a single file, while also not hampering further extendibility in the future.

The format employs a variety of packet types, representing different data regarding an object. Each packet begins with a header, specifying the type of the packet and making it easy to identify and process the packet appropriately. This is one such packet:
```json
{"type": 2}
{"frame": 0, "elapsedTime": 0.0}
```
In this packet he first line is the header indicating the packet type (2, or `FramePacket`), followed by the packet's data.

**Important Note:** packets will always be written in the order in which they were emitted.

## Processing the Packet data
### C# API Example

For C# users, the packet-based format can be handled as follows:

```csharp
using EventHorizon;

class Program {
  void Main() {
    // Load the EVH recording and parse packets
    using var fs = File.Open(ctxAssetPath, FileMode.Open);
    using var reader = new FormatReader(fs);

    var metadataPacket = reader.ReadPacket<MetadataPacket>();
    while (!reader.IsEndOfFile)
    {
      switch (reader.ReadPacket())
      {
        case TransformPacket transformPacket:
           // Process transform packet
          break;
        case ActivationPacket activationPacket:
          // Process activation packet
          break;
        // Additional cases for other packet types
      }
    }
	}
}
```

This approach enables seamless integration and manipulation of recording data within Unity projects. However, usage outside the Unity environment is currently not verified and may not be supported. This is primarily due to the reliance on Unity's `JsonUtility` API for implementing serializing/deserializing and the `SerializeField` attribute. While the data classes are standard .NET code, they utilize the `SerializeField` attribute, which is specific to Unity. For non-Unity contexts, this attribute might require replacement with an alternative or a custom implementation to ensure compatibility.

### Python example

Since EVH files encapsulate JSON Lines data compressed with Brotli, the following example code illustrates a straightforward and minimal method to decompress and read EVH files:

```python
import brotli  # pip install brotli
import json

with open("path/to/recording_file.evh", "br") as file:
    decompressed = brotli.decompress(file.read()).decode('utf-8')

    # Split the decompressed data into lines
    lines = decompressed.splitlines()

    # Iterate through the lines in pairs (header, packet)
    for i in range(0, len(lines), 2):
        header_line = lines[i]
        packet_line = lines[i + 1]

        # Parse and print each header and packet
        if header_line and packet_line:  # Check if lines are not empty
            header = json.loads(header_line)
            packet = json.loads(packet_line)
            print(header)
            print(packet)
```

### Other languages
Assuming you have a brotli decompressor and a json parser, you can use the provided information to parse (or discard) the packets, at your discretion.

## Detailed overview of packet types

### Metadata Packet

Contains essential metadata about the recording.

- `version`: Recording format version.
- `sceneName`: Name of the scene being recorded.
- `fps`: Frame rate at which the recording was made.
- `timestamp`: The date and time when the recording was made.

**Example:**
```json
{"type": 1}
{
    "version": 2,
    "sceneName": "ExampleScene",
    "fps":
    {
        "numerator": 30,
        "denominator": 1
    },
    "timestamp": "2024-01-08T12:00:00Z"
}
```

### VR Metadata Packet

Contains metadata specific to Virtual Reality environments. This might be extended in the future.

- `headsetType`: Type of VR headset used.
- `interactionProfile`: Interaction profile for the VR setup.

**Example (expanded for readability):**
```json
{"type": 5}
{
    "headsetType": "Model X",
    "interactionProfile": "Profile Y"
}
```

### Frame Packet

Marker packet for the start of a new frame. Currently has no use internally, and is meant for users.

- `frame`: Frame number.
- `elapsedTime`: Time elapsed since the beginning of the recording.

**Example (expanded for readability):**
```json
{"type": 2}
{
    "frame": 123,
    "elapsedTime": 1.033
}
```

### Transform Packet

Describes the transformation (position, rotation, scale) of an object at a specific frame.

- `frame`: Frame number.
- `id`: Unique identifier for the object.
- `isLocal`: Boolean indicating if the transformation is local.
- `translation`: Object's position in 3D space.
- `rotation`: Object's rotation represented as a quaternion.
- `scale`: Object's scale in 3D space.

**Example (expanded for readability):**
```json
{"type": 3}
{
    "frame": 123,
    "id": 1,
    "isLocal": true,
    "translation": {
        "x": 1.0,
        "y": 2.0,
        "z": 3.0
    },
    "rotation": {
        "x": 0.0,
        "y": 0.7071,
        "z": 0.0,
        "w": 0.7071
    },
    "scale": {
        "x": 1.0,
        "y": 1.0,
        "z": 1.0
    }
}
```

### Generic Data Packet

For storing arbitrary data related to a specific frame.

- `frame`: Frame number.
- `data`: A string containing the generic data.

**Example (expanded for readability):**
```json
{"type": 4}
{
    "frame": 123,
    "data": "Custom data here"
}
```

### Activation Packet

Used to mark the activation of certain game objects. Given ActivationTrackable, when this packet will appear, the game object with the given trackable will be enabled, and otherwise it would be disabled.

- `frame`: Frame number at which the activation occurs.
- `id`: Identifier for the activation event.

**Example (expanded for readability):**
```json
{"type": 6}
{
    "frame": 123,
    "id": 2
}
```
