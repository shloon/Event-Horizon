<h1 align="center">
    <img src="Packages/il.runiarl.eventhorizon/Documentation~/images/event-horizon-logo.png" width="180"/>
    <br>
    Event Horizon
    <br>
</h1>

<p align="center">
    Utilities for recording, reproducing, and visualizing VR experiments in <a href="https://unity.com">Unity</a>. <br/> <a href="Packages/il.runiarl.eventhorizon/Documentation~/">See documentation on Github</a>.
</p>

<p align="center">
    <img alt="Shloon/Event-Horizon" src="https://img.shields.io/badge/shloon-event--horizon-2794d9?style=for-the-badge" />
    <img alt="GitHub License" src="https://img.shields.io/badge/License-MIT/APACHE-548ca4?style=for-the-badge" />
    <img alt="GitHub CI Status" src="https://img.shields.io/github/actions/workflow/status/shloon/Event-Horizon/ci.yml?style=for-the-badge">
    <img alt="GitHub issues" src="https://img.shields.io/github/issues/Shloon/Event-Horizon?style=for-the-badge" />
</p>

## Getting started
See [GETTING_STARTED.md file in the project's documentation.](Packages/il.runiarl.eventhorizon/Documentation~/LIMITATIONS.md).

Alternatively, you can checkout our video tutorials:

<p align="center">
    <a href="https://www.youtube.com/watch?v=Y-a6nUdYZCs"><img alt="Part 1 Thumbnail" src="https://img.youtube.com/vi/Y-a6nUdYZCs/maxresdefault.jpg" width="350"/></a>
    <a href="https://www.youtube.com/watch?v=Dtpcvc3PMVg"><img alt="Part 2 Thumbnail" src="https://img.youtube.com/vi/Dtpcvc3PMVg/maxresdefault.jpg"  width="350"/></a>
</p>

## Installation

### Via Official Scoped Repository
**Note**: Event Horizon currently requires the use of preview packages, which may contain untested beta features. Stick to non-preview releases unless advised otherwise or if you know what you're doing.

We currently host all Event Horizon packages on NPM with the `il.runiarl` prefix. Here's how to add our scoped registry:
1. Go to the `Package Manager` in `Project Settings`.
2. Add a new scoped registry with the following details:
   - Name: `ARL EventHorizon`
   - URL: `https://registry.npmjs.com`
   - Scope(s): `il.runiarl`
3. Click on `Save`.


<img src="Packages/il.runiarl.eventhorizon/Documentation~/images/scoped-registry-01.png" alt="Scoped registry settings illustration" width="540"/>

Next, open the Package Manager window, click on the dropdown label menu labeled "Packages:", and select `My Registries` to list our packages.

<img src="Packages/il.runiarl.eventhorizon/Documentation~/images/scoped-registry-02.png" alt="Scoped registry selection dropdown illustration" width="200" />

We advise only installing the packages relevant to your development environment. For instance, when working with the Meta XR SDK, opt for the `Event Horizon MetaXR Integration` package; it will automatically handle the dependencies for you.

### Via Git URL
Alternatively, you can add `Packages/manifest.json` with your favorite text editor. For example, to add the core package (`il.runiarl.eventhorizon`), add following line to the dependencies block:

```json
"dependencies": {
  "il.runiarl.eventhorizon": "https://github.com/shloon/Event-Horizon#upm",
  ...
}
```

This method requires manual management of package versions. Check the Event Horizon GitHub repository for the latest compatible releases. For more information, read the Packages section below.

## Packages
This repository serves as a comprehensive collection of all packages necessary for the Event Horizon suite. Below is an overview of the primary packages and their purposes.

| Package                          | Description                                                                                    | Branch       |
| -------------------------------- | ---------------------------------------------------------------------------------------------- | ------------ |
| `il.runiarl.eventhorizon`        | The foundational package required for any Event Horizon implementation.                        | `upm`        |
| `il.runiarl.eventhorizon.metaxr` | Dedicated package for integration with the Meta XR SDK, enhancing support for MetaXR hardware. | `upm-metaxr` |

Officially released branches are distinctly marked with their version numbers, for instance, `upm-v0.1.0` denotes the version for the `il.runiarl.eventhorizon` package.

The branches specified are consistently updated with the latest changes whenever commits are pushed to the repository. However, in order to access the most recent updates, you need to manually update your packages, as the Unity package manager does not handle this process automatically. Consult [Unity's documentation on Locked Dependencies](https://docs.unity3d.com/Manual/upm-git.html#git-locks) for more information.

it is recommended to stay informed about updates and changes, as we may add, change or deprecate some of the packages listed above.

# Limitations

See the [LIMITATION.md file in the project's documentation.](Packages/il.runiarl.eventhorizon/Documentation~/LIMITATIONS.md).

# Future Improvements

See the [Relevant issue](https://github.com/shloon/Event-Horizon/issues/2).
