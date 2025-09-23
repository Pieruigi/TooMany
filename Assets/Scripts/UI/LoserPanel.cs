using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace TMOT.UI
{
    public class LoserPanel : MonoBehaviour
    {
        [SerializeField]
        TMP_Text msgTextField;

        [SerializeField]
        TMP_Text actionTextField;

        List<string> texts;

        string actionStrFormat = "Reset in {0}.";

        bool loop = false;

        float counter = 3;

        void Awake()
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!loop) return;

            counter -= Time.unscaledDeltaTime;

            actionTextField.text = string.Format(actionStrFormat, Mathf.Ceil(counter));   
        }

        void OnEnable()
        {
            loop = true;
            counter = 3;
            FillList();
            msgTextField.text = texts[UnityEngine.Random.Range(0, texts.Count)];
            actionTextField.text = string.Format(actionStrFormat, counter);
        }

        void OnDisable()
        {
            msgTextField.text = "";
            loop = false;
        }

        private void FillList()
        {
            texts = new List<string>
            {
                "Wow... that was fast.",
                "Bots 1 - You 0.",
                "Were you even trying?",
                "Hunter? More like lunch.",
                "Prey forever, hunter never.",
                "Restarting... maybe you'll last longer this time.",
                "Reflexes expired.",
                "Did the bots even break a sweat?",
                "Try again. The bots are bored.",
                "That pill didn't help much, huh?",
                "Game over. Again.",
                "Hint: survive longer.",
                "Blink and you're dead.",
                "The experiment is disappointed.",
                "Retrying... because that was embarrassing.",
                "Ouch... bots win again.",
                "You call that survival?",
                "Even the tutorial bots are laughing.",
                "That was... impressively bad.",
                "Survival time: short. Very short.",
                "Did you trip on your own reflexes?",
                "The bots say thanks for the snack.",
                "Hunter instincts? Missing.",
                "Maybe slower speed would help... oh wait.",
                "Blink slower next time.",
                "Your controller is fine, your skills... not so much.",
                "The bots didn't even notice you were here.",
                "Retry. Maybe actually survive this time.",
                "Your prey phase lasted forever.",
                "Hunter mode? Never heard of it.",
                "And just like that, you're gone.",
                "The bots are undefeated.",
                "Quick death, quick restart.",
                "Try again... with effort, maybe?",
                "That was over before it started.",
                "Wow... did you even try?",
                "Fastest fail of the day!",
                "The bots didn't even break a sweat.",
                "Hunter? More like lunch.",
                "Is this your personal best? Yikes.",
                "Reflexes of a potato detected.",
                "You survived about... zero seconds.",
                "Speedrun to death complete!",
                "Did you just hand them the win?",
                "Even Pac-Man lasted longer.",
                "Bots: 1, You: 0 (again).",
                "That was embarrassing to watch.",
                "You play, they laugh. Great system.",
                "Next time, maybe dodge?",
                "Your 'survival horror' was just horror.",
                "Bots are evolving. You're... not.",
                "Hunter instincts? Still loading...",
                "Retry, but don't blink this time.",
                "Was that supposed to be impressive?",
                "The bots thank you for feeding them.",
                "Congratulations, you unlocked... disappointment!",
                "Pro gamer move: uninstall?",
                "Achievement unlocked: Instant Regret.",
                "Error 404: Skill not found.",
                "Have you tried... not dying?",
                "New record! For dying the fastest.",
                "Tip: The bots aren't supposed to win.",
                "Plot twist: You were the tutorial boss.",
                "Game over sponsored by… you.",
                "Guess who just became bot food?",
                "Keyboard broken? Or just you?",
                "Don't worry, even the bots are laughing.",
                "Next run: maybe use both hands.",
                "Your ancestors are proud… of the bots.",
                "That was a bold strategy. It failed.",
                "Hint: Walls aren't enemies.",
                "Even the main menu is harder than you.",
                "Bots voted: you're our favorite snack.",
                "Pro tip: survival usually lasts longer.",
                "That's one way to speedrun the quit button."
            };

        }
    }
}