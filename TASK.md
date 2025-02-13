

# Patch-LSD (Leave Stay Decision)

This document describes the key points behind this implementation of the Patch-LSD (Leave Stay Decision) task.


## The Task
The task is designed to elucidate switching behaviour as a function of time linked reward rates. For additional context, read the <a  href="https://doi.org/10.1038/ncomms12327"  target="_blank">original paper</a> and play a few rounds of the task in the <a  href="https://i-brnrd.github.io/patchLSD/"  target="_blank">demo version</a>.

**Overview**\
Participants are presented with different patches that present rewards at different rates.\
Patches consist of a coloured box. In non-reward events, this box is empty, and in reward events, the box is filled to a given level with an illustration of gold bars.\
The Blue box indicates a patch with a changing reward rate (i.e. the rate changes with timestep). The Red box indicates a stable (default) reward rate.\
After a (varying) number of events in a Blue box patch, participants are asked to decide whether to stay in that Blue box patch, or leave to go to a Red box patch (with a stable reward rate that participants have learned during training).

On load, the Main Menu will appear. Two modes are available to the experimenter or participant; Training (A, B & C) & Task.\
If training mode is selected, no data is stored or written out.

On devices, the task can be paused and restarted via <kbd>Esc</kbd>. On the WebGL version, clicking away from the browser will pause- to return to the main menu; just refresh the browser.

## Training
No data is stored/ written out to files in the training sessions. Pausing via <kbd>Esc</kbd> allows return to main menu,  no training progress is saved.

The aim of the training is to familiarise the participant with the task, and with the stable reward rate in the default (red) environment.\
The training options A, B & C are as set out in the <a  href="https://doi.org/10.1038/ncomms12327"  target="_blank">original paper's</a> supplementary material- <a  href="https://static-content.springer.com/esm/art%3A10.1038%2Fncomms12327/MediaObjects/41467_2016_BFncomms12327_MOESM821_ESM.pdf"  target="_blank"> see here</a>.

The instructions LINK LINK LINK provided give experimenters more detailed descriptions of ow to to deliver the training.

### Training A:
(see LINK LINK LINK training A controller)
Consists of 10 length-matched patches (15 each).
First Red (default) is presented, then Blue (changing).
After this, a decision screen is presented to ask whether
The patches are the selected from Marco's raw data (LINK LINK LINK) to give a good mix of
    length matched blue (15); pause, then red (15)

    int[] trialsA = { 17, 41, 85, 26, 64, 38, 75, 3, 12, 52 };

Training A runs 3 times

At the end of t

### Training B:
3 x
    3 repeats (?) of a full blue patch; i.e rew2ld + stay. Which ine?


### Training C:
18 trials of task + POINTS displayed (fixed pattern)



## Task:
90 trials drawn from data provided by Marco.

Each trial is

Blue patch for 15/16/17 events.

Events are reward or non reward (drawn from Marco's task data*)

Leave-Stay Decision

Either

-Stay in Blue patch

-Leave for Red patch (default)

Each quarter of experiment, subjects recieve feedback about Bonus Points- done

60% of trials were truncated after the LSD- done?

We need to decide whether orders of these should be fixed or not and



* Patches drawn from same reward rate curve must not be presented concurrently
