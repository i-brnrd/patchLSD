
# Patch-LSD

Unity implementation of  <a  href="https://doi.org/10.1038/ncomms12327"  target="_blank">Marco Wittman's Patch Foraging Task</a>:

![alt text](Assets/Resources/Screenshots/combo.png)


<small> A reinforcement learning foraging task developed to elucidate switching behaviour as a function of time linked reward rates.

## Description
Intended to be a portable-ish OS/ device agnostic version of this task; to allow diverse teams (clinical, academic) to collect data easily. This version also contains optional EEG streaming via  <a  href="https://labstreaminglayer.readthedocs.io/index.html"  target="_blank">LSL's</a> (Lab Streaming Layer) custom Unity package,  <a  href="https://github.com/labstreaminglayer/LSL4Unity"  target="_blank">LSL4Unity</a>.

## Notes
C# code/ art is very basic (no C# or game expertise on team).\
Builds tested on for webGL, Mac, Windows & Linux (the EEG LSL stream has only been tested on Windows). As of Feb 2025 iOS/android not tested.

A WebGL demo version is available <a  href="https://i-brnrd.github.io/patchLSD/"  target="_blank">here</a>; however this does not include options for meaningful data collection or EEG streams.

### Task
See [here](/TASK) for details on the actual task and programmatic implementation.

## Getting Started
If you want to build a version for use in your lab; Download Unity (& dependencies); download this repo & build.\
If you just want to run through the task, a web demo version is available <a  href="https://i-brnrd.github.io/patchLSD/"  target="_blank">here</a>;

### Dependencies

 <a  href="https://unity.com/releases/editor/whats-new/2022.3.16"  target="_blank">Unity 2022.3.16f1</a> (2D: URP)\
  <a  href="https://labstreaminglayer.readthedocs.io/index.html"  target="_blank">LSL4Unity</a> (Lab Streaming Layer as a Unity package)


### Logs, Data
UPDATE THIS

Behavioural data (see [here](/TASK) for more detail) is written to (device specific) [Unity Application Persistent Data Path](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html). Any issues accessing this please get in touch.
WebGl results are written onto the browser console. Looking to improve this in a low tech way (eg just read out something sensible to idbfs).

## Help
Feel free to contact [@i-brnrd](https://github.com/i-brnrd) on here or via [University of Dundee](https://www.dundee.ac.uk/people/isla-barnard).

## Authors
Isla Barnard (developer)\
Mihaela Lyustkanova (designed EEG integration)\
[Marco Wittman](https://www.wittmann-lab.com/contact) (original concept)\
Tom Gilbertson (lead)

## Citation
If you use this you must cite the original paper:

Wittmann, M., Kolling, N., Akaishi, R. et al. Predictive decision making driven by multiple time-linked reward representations in the anterior cingulate cortex. Nat Commun 7, 12327 (2016). https://doi.org/10.1038/ncomms12327

Please consider also citing this repo:

## Acknowledgments
To [Marco Wittman](https://www.wittmann-lab.com/contact) for <a  href="https://doi.org/10.1038/ncomms12327"  target="_blank">Predictive decision making driven by multiple time-linked reward representations in the anterior cingulate cortex</a>

## License (MIT)
See [here](/LICENSE).
