using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rnd = UnityEngine.Random;
using KModkit;
public class BaybayinWords : MonoBehaviour {
    public TextMesh display;
    public KMSelectable[] buttons;
    string[] wordsEnglish = new string[] { "Dove", "House", "Food", "Sleep", "Bridge", "Wood", "Sugar", "Rich", "Shoe", "Gold", "Brown", "Treasure", "Weak", "Guava", "Happy", "Thanks", "Captain", "Clap", "Eyes", "Dive", "Plant", "Medicine", "Island", "Hide", "Swim", "Fly", "Lightning", "Dinner", "Breakfast", "Lunch", "Clothing", "Copper", "Silver", "Dead", "Deaf", "Blind", "Lame", "Mute", "Stain", "Plate", "Knife", "Necklace", "Ring", "Stop", "Walk" };
    string[] wordsBaybayin = new string[] { "ᜃᜎᜉᜆᜒ", "ᜊᜑᜌ᜔", "ᜉᜄ᜔ᜃᜁᜈ᜔", "ᜆᜓᜎᜓᜄ᜔", "ᜆᜓᜎᜓᜄ᜔", "ᜆᜓᜎᜓᜌ᜔", "ᜃᜑᜓᜌ᜔", "ᜀᜐᜓᜃᜎ᜔", "ᜋᜌᜋᜈ᜔", "ᜐᜉᜆᜓᜐ᜔", "ᜄᜒᜈ᜔ᜆᜓ", "ᜃᜌᜓᜈ᜔ᜄᜒ", "ᜃᜌᜋᜈ᜔ᜀᜈ᜔", "ᜊᜌᜊᜐ᜔", "ᜋᜐᜌ", "ᜐᜎᜋᜆ᜔", "ᜃᜉᜒᜆᜈ᜔", "ᜉᜎᜃ᜔ᜉᜃ᜔", "ᜋᜆ", "ᜐᜒᜐᜒᜇ᜔", "ᜑᜎᜋᜈ᜔", "ᜄᜋᜓᜆ᜔", "ᜁᜐ᜔ᜎ", "ᜆᜄᜓ", "ᜎᜅᜓᜌ᜔", "ᜎᜒᜉᜇ᜔", "ᜃᜒᜇ᜔ᜎᜆ᜔", "ᜑᜉᜓᜈᜈ᜔", "ᜀᜄᜑᜈ᜔", "ᜆᜅ᜔ᜑᜎᜒᜀᜈ᜔", "ᜇᜋᜒᜆ᜔", "ᜆᜈ᜔ᜐᜓ", "ᜉᜒᜎᜃ᜔", "ᜉᜆᜌ᜔", "ᜊᜒᜅᜒ", "ᜊᜓᜎᜄ᜔", "ᜉᜒᜎᜃ᜔", "ᜉᜒᜉᜒ", "ᜋᜈ᜔ᜆ᜔ᜐ", "ᜉ᜔ᜎᜆᜓ", "ᜃᜓᜎ᜔ᜆ᜔ᜐᜒᜎ᜔ᜌᜓ", "ᜃᜓᜏᜒᜈ᜔ᜆᜐ᜔", "ᜐᜒᜅ᜔ᜐᜒᜅ᜔", "ᜑᜒᜈ᜔ᜆᜓ", "ᜎᜃᜇ᜔" };
    List<string> moduleWords;
    int correctWordIndex;
    public KMBombModule module;
    public KMAudio sound;
    int moduleId;
    static int moduleIdCounter = 1;
    bool solved;
    void Awake()
    {
        moduleId = moduleIdCounter++;
        for (int i = 0; i < 6; i++)
        {
            buttons[i].OnInteract += PressButton(i);
        }
        correctWordIndex = rnd.Range(0, 6);
        module.OnActivate += ActivateModule;
    }
    void ActivateModule () {
        moduleWords = wordsEnglish.ToList().Shuffle().Take(6).ToList();
        for (int i = 0; i < 6; i++) buttons[i].GetComponentInChildren<TextMesh>().text = moduleWords[i];
        display.text = wordsBaybayin[Array.IndexOf(wordsEnglish, moduleWords[correctWordIndex])];
        Debug.LogFormat("[Baybayin Words #{0}] The word on the screen is {1}.", moduleId, moduleWords[correctWordIndex]);
	}
	KMSelectable.OnInteractHandler PressButton (int index) {
        return delegate
        {
            if (!solved)
            {
                buttons[index].AddInteractionPunch();
                sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                Debug.LogFormat("[Baybayin Words #{0}] You pressed the button labeled {1}.", moduleId, moduleWords[index]);
                if (index == correctWordIndex)
                {
                    sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                    Debug.LogFormat("[Baybayin Words #{0}] That was correct. Module solved.", moduleId);
                    display.text = "";
                    module.HandlePass();
                    solved = true;
                }
                else
                {
                    module.HandleStrike();
                    Debug.LogFormat("[Baybayin Words #{0}] That was incorrect. Strike!", moduleId);
                    correctWordIndex = rnd.Range(0, 6);
                    ActivateModule();
                }
            }
            return false;
        };
	}
#pragma warning disable 414
    private string TwitchHelpMessage = "use '!{0} 1/2/3/4/5/6' to press the Nth button in reading order.";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant();
        string validcmds = "123456";
                if (!validcmds.Contains(command[0]))
                {
                    yield return "sendtochaterror Invalid command.";
                    yield break;
                }
                for (int j = 0; j < 6; j++)
                {
                    if (command[0] == validcmds[j])
                    {
                        yield return null;
                        buttons[j].OnInteract();
                    }
                }
    }
    IEnumerator TwitchHandleForcedSolve()
    {
        yield return null;
        buttons[correctWordIndex].OnInteract();           
    }
}
