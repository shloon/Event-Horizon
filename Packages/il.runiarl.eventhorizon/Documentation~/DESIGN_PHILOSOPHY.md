## Design Philosophy
EventHorizon is developed with a commitment to simplicity, ease-of-use, accuracy, and efficiency, and aims to provide an accessible and straightforward tool for novice Unity users and researchers. As such, it may include some design decisions that might seem weird at first glance; However, they are all done to balance these principles. In this document, we will give an overview of some of these decisions, without getting into too much detail.

### Simplicity, Ease of Use, and Idiot-proofing
- EventHorizon is an extension of Unity
- It is made to be as simple as possible, to save researcher's time on coding custom-made solutions for the same issue (recording experimental data)
- It allows researchers to focus on conducting the experiment.
- It should have the least friction possible.
- To that end, we aim to have it as a natural part of the editor:
  - integrating to familiar workflows that even novice users use, such as adding components to game objects and using prefabs, following their intuition about the engine
  - create inspectors and other tools that make working with our custom data easy while still preventing mistakes.
- We additionally write tools to help prevent the users of this library from doing common mistakes, and even possibly fix them.
- Simple data format for recordings, using commonplace technologies (json, brotli), that can be easily analyzed for data-science purposes.

### Accuracy and Reproducibility
- The accuracy of both the recording process and the playback process is of upmost importance.
- We refer mostly to the accurate reproduction of the objects in the scene, as well as their positions, with timed information covering changes in these aspects and more.
- Furthermore, reproducibility is very important. Two senses of reproducibility:
- First sense of reproducibility - the ability to exactly reproduce how the scene was played back based on the recording, using the same scene, same ids for objects, and more
- Second sense of reprodcibility - use commonplace formats to make sure researchers can work with the data in whatever platform they wish.

### Efficiency
- Performance is a key concern.
- We strive to reduce runtime performance penalty as much as possible.
- On the main thread, runtime performance > RAM usage in most cases, as long as there no impact on the accuracy of recorded data.
- In performance-critical areas, performance is much more important than clean, DRY and SOLID code.
- We will aim to perform computationally-expensive calculations on other threads, or before the game starts if multithreading is not an option.
- We currently do NOT use jobs, as we did not find sufficient justification for their runtime overhead.

## Testability
- To ensure the longevity and correctness of the library, every aspect of the library must be tested, preferably in an automated fashion.
  - This usually means writing unit, integration and performance tests.
- Ideally, most workflows must be somewhat unit-testable via Unity Runtime tests.
- We try to refactor as much of the code out of the editor dll into testable classes and methods.
- However, some of the components change or add functionality that isn't directly unit-testable (such as custom property drawers, automated layout changing, resilience against domain reloading, and so on). As the testing documentation on these is sparse, they require some innovative testing procedures that aren't 100% fully compatible with the Unity way of doing things. In these cases, testability of the code is a much greater concern than the "unity-esque"-ness of the code, so long as it doesn't result in a conceivable impact for the library's users. When in doubt, reference the existing testing code. Of course, if you have a way to make things more testable in a cleaner fashion, please do feel free to open a pull request and improve the existing state of affairs.

### Third-party dependency usage
To simplify the deployment of the library, we will strive to use the least amount of dependencies, especially third party libraries. The use of third-party UPM packages, such as those on OpenUPM, is discouraged, as it would make deploying the app much more difficult. However, there are some exceptions, which currently include:
 - Unity-made dependencies that we can automatically fetch from them (such as the `Unity Timeline` and `Unity UI` packages)
 - VR-related core libraries (such as the `Meta XR SDK Core` package)
 - Small, pure .NET libraries that contain functionality outside the scope of the package. (such as PcgRng)
 - Test packages that can be natively pulled by Unity (such as `nuget.moq`, or the built-in `NUnit`)
