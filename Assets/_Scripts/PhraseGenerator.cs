using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

    public class PhraseGenerator : Singleton<PhraseGenerator>
    {
        [SerializeField] private int detailPercentage = 20;
        
        public void Start()
        {
            int seed = (int)DateTime.Now.Ticks;
            Random.InitState(seed);
        }

        public String GeneratePhrase(Person person)
        {
            string personName = person.Name;
            string[] personalityTraits = person.Traits.Select(x => x.TraitName).ToArray();

            string description = GenerateDescription(personName, personalityTraits, detailPercentage); 
            Debug.Log(description);

            return description;
        }

        private string GenerateDescription(string personName, string[] personalityTraits, int detailChance)
        {
            // string description = personName + " is ";
            //
            // int numTraits = personalityTraits.Length;
            // for (int i = 0; i < numTraits; i++)
            // { 
            //     if (i == numTraits - 1) 
            //         description += "and ";
            //     
            //     description += GetQuantifier(numTraits) + "" + personalityTraits[i];
            //     if (i < numTraits - 1)
            //         description += ", ";
            //     
            //     if (Random.Range(1, 101) <= detailChance)
            //     {
            //         if (i == numTraits - 1)
            //             description += ", ";
            //         description += AddDetails(personalityTraits[i]);
            //         if (i < numTraits - 1)
            //             description += "; ";
            //     }
            // }
            //
            // description += ".";
            //
            // return description;
            
            string description = personName + " is ";

            int numTraits = personalityTraits.Length;
            bool startOfPhrase = true;
            for (int i = 0; i < numTraits; i++)
            {
                if (i == numTraits - 1 && !startOfPhrase)
                    description += "and ";
                description += (Random.Range(1, 101) <= detailChance ? GetQuantifier(i) : "") + personalityTraits[i];
                startOfPhrase = false;
                if (i < numTraits - 1)
                    description += ", ";
            }

            description += ".";
            bool startOfDetail = true;
            List<string> details = new List<string>();
            for (int i = 0; i < numTraits; i++)
            {
                if (Random.Range(1, 101) <= detailChance) 
                {
                    if (startOfDetail)
                        description+= " He";
                    details.Add(AddDetails(personalityTraits[i]));
                    startOfDetail = false;
                }
            }

            if (details is { Count: > 1 })
            {
                List<string> withSeparator = new List<string>();

                foreach (var detail in details)
                {
                    withSeparator.Add(detail + "; ");
                }
                
                String lastDetail = withSeparator[^1];
                withSeparator[^1] = "and";
                withSeparator.Add(lastDetail);

                foreach (var detailWithSeparator in withSeparator)
                {
                    description += detailWithSeparator;
                }
            }
            else if (details.Count > 0)
            {
                description += details[0];
            }
            
            description = description.TrimEnd();

            char lastChar = description[^1];
            
            if (lastChar == ';')
            {
                //description = (string)description.Take(description.Length-2).ToString() + ".";
                Char[] desc = description.ToCharArray();

                desc[^1] = '.';

                description = new string(desc);

            }
            else if (lastChar != '.')
            {
                description += '.';
            }
            
            return description;
        }

        private string GetQuantifier(int index)
        {
            string[] quantifiers = { "extremely ", "very ", "quite ", "a little " };

            if (index >= quantifiers.Length)
            {
                return "";
            }
            
            return quantifiers[index];
        }

        private string AddDetails(string trait)
        {
            switch (trait)
            {
                case "Anxious":
                    return " is often worried about the future and experiences frequent concerns";
                case "Assertive":
                    return " knows how to express his opinions and defend his rights confidently";
                case "Curious":
                    return " is always eager for knowledge and constantly seeks to learn new things";
                case "Empathetic":
                    return " truly understands what others feel and knows how to put himself in their shoes";
                case "Insensitive":
                    return " often appears indifferent to others' feelings and can come across as cold";
                case "Judgmental":
                    return " tends to evaluate and comment on others' actions in a harsh manner";
                case "Level-Headed":
                    return " keeps his cool even in the most stressful and challenging situations";
                case "Optimistic":
                    return " always sees the glass half full and finds the positive side of things";
                case "Pessimistic":
                    return " tends to view things negatively and anticipates failure";
                case "Tolerant":
                    return " accepts others' differences and is open-minded";
                case "Sad":
                    return " often feels sorrow and unhappiness";
                case "Happy":
                    return " is usually cheerful and content";
                case "Disgusted":
                    return " has a strong feeling of dislike or revulsion";
                case "Angry":
                    return " easily becomes mad or annoyed";
                case "Surprised":
                    return " feels amazed or astonished";
                case "Fearful":
                    return " is often afraid about what might happen";
                default:
                    return "";
            }
        }
    }
