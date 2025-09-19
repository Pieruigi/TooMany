using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TMOT.UI
{
    public class WinnerPanel : MonoBehaviour
    {

        [SerializeField]
        TMP_Text msgTextField;

        [SerializeField]
        TMP_Text actionTextField;

        List<string> texts;

        string actionStrFormat = "Next run in {0}.";

        bool loop = false;

        float counter = 5;
     
        
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
            counter = 5;
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
                "Yeah, the bots were totally asleep...",
                "Not bad… but nothing to brag about.",
                "Wow, you won… against bots.",
                "Congrats! But the bots were obviously texting each other.",
                "You won, but don't get cocky.",
                "Nice try… was that easy mode?",
                "Sure, you beat it. Must have been the tutorial.",
                "Okay, you won. Now go back to losing as usual.",
                "Not terrible… the bots just let you through.",
                "Victory! But the bots already asked for a rematch.",
                "Big deal… pretty sure it was just a glitch.",
                "You actually won… somehow.",
                "Victory! But let's not throw a parade.",
                "Come on, even a toaster could've done that.",
                "Fine, you survived… this time.",
                "You won, but let's be honest, it was luck.",
                "Congrats, now go tell your mom.",
                "You beat the bots… but they respawn anyway.",
                "Champion of nothing! But hey, good job.",
                "You won, but only because the bots felt sorry for you.",
                "Wow, even the bots are impressed… barely.",
                "You won… congratulations, I guess.",
                "Easy peasy… or were the bots just lazy?",
                "Victory! But let's not overdo it.",
                "Not too shabby… for a human.",
                "You made it… but only just.",
                "Hah! You won… but don't let it get to your head.",
                "Well done… the bots were very polite.",
                "You survived… barely. Try not to die again.",
                "Winner… kind of. The bots were distracted.",
                "Yay, you did it… almost miraculous.",
                "Congrats! But the bots were not trying hard.",
                "Victory unlocked… but it wasn't that epic.",
                "You beat it… against minimal resistance.",
                "Nice job… the bots were taking a nap.",
                "Winner winner… bot-free dinner?",
                "You got through… barely scraping victory.",
                "Success! But the bots are laughing behind your back.",
                "Well… you survived. For now.",
                "Champion status… but only in this run."
            };

        }
    }
}