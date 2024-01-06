# Limitations

Below are the currently known limitations of Event Horizon.

- Cannot dynamically add object to scene - timeline doesn't support this (TODO MVP)
- Recording across multiple scenes is currently not supported due to issue above.
- No support for a standalone mode (e.g. running from mac)
- Trackable IDs must be claimed for the duration of the program's lifetime

## Meta-XR Specific Limitations
- Cannot change to a different kind of controller mesh throughout experiment.
- Cannot reproduce hand-to-controller switches and vice versa (TODO).
- Currently only Oculus Quest 2 Controllers and hand tracking are supported. Other controllers may be supported due to testing in emulator.