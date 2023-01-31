/**
 * Project Ritmania
 * 
 *      ::Overall Structure::
 *      
 *          - GameManager:
 *              Sets the Target FPS value on start.
 *              Can toggle the AutoPlay function by pressing "A". 
 *      
 *          - BeatMapCaller:
 *              Responsible for initiating all the systems necessary to play a beatmap. 
 *              Initiates Conductor and BeatTrack.
 *      
 *          - Conductor:
 *              Responsible for counting the song time, as well as it's BPM.
 *              It's initiated and receives data from BeatMapCaller when the gameplay starts.
 *             
 *          - BeatTrack:
 *              Handles the spawning cycle of HitNotes, as well as the screen path that they move on.
 *              All Hit Notes are pre-loaded on a ObjectPool attached to the component.
 * 
 *          - HitNote:
 *              The notes that should be hit during the gameplay.
 *              They are physically lerped between the spawn point and the reaching point of the BeatTrack.
 *              When the AutoPlay flag is active on the Game Manager, it hits itself as soon as it can.
 *              
 *          - RhythmJudge:
 *              Handles the player interaction with the game. 
 *              Reads inputs and compares the hit timestamp to output a PrecisionScore.
 *              PrecisionScores can be Perfect, Good, Bad or Miss. 
 *              
 **/