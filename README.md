
# Patch-LSD

Unity implementation of  <a  href="https://doi.org/10.1038/ncomms12327"  target="_blank">Marco Wittman's Patch Foraging Task</a>; a reinforcement learning foraging task developed to elucidate switching behaviour as a function of time linked reward rates.

## Description
Intended to be a portable-ish OS/ device agnostic task; but it is now used for EEG recording.
Builds work for for webGL, Mac & Windows- however the LSL plugin (used for eeg) as only been tested on Windows.
A WebGL demo version is available <a  href="https://i-brnrd.github.io/patchLSD/"  target="_blank">here</a>; however this does not include options for meaningful data collection or EEG streams.

iOS not tested.

### Task
See See [here](/TASK).

## Getting Started
To build; download Unity (see dependencies); and import & build
C# code is basic and  verbose (no advanced features of the C# language)

### Dependencies
Unity 2022.3.16f1; 2D (URP)

### Builds & Installing
Follow Unity Build Processes but just ask if help needed; always happy.

### Logs, Data
Data read out to the device specific [Unity Application Persistent Data Path](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html); WebGl results are written onto the browser console. Looking to improve this in a low tech way (eg just read out something sensible to idbfs).

## Help
For code/ build [@i-brnrd](https://github.com/i-brnrd) on here or via [University of Dundee](https://www.dundee.ac.uk/people/isla-barnard).

## Authors
Isla Barnard (developer)

[Marco Wittman](https://www.wittmann-lab.com/contact)

Mihaela Lyustkanova, Tom Gilbertson (lead)

## Acknowledgments
To [Marco Wittman](https://www.wittmann-lab.com/contact) for <a  href="https://doi.org/10.1038/ncomms12327"  target="_blank">Predictive decision making driven by multiple time-linked reward representations in the anterior cingulate cortex</a>

## License
See [here](/LICENCE).
