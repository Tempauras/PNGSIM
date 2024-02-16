using System;
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
            int _seed = (int)DateTime.Now.Ticks;
            Random.InitState(_seed);
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
            for (int i = 0; i < numTraits; i++)
            {
                if (Random.Range(1, 101) <= detailChance) 
                {
                    if (startOfDetail)
                        description+= " He";
                    if (i == numTraits - 1 || !startOfDetail)
                        description += " and ";
                    description += AddDetails(personalityTraits[i], startOfDetail);
                    startOfDetail = false;
                    if (i < numTraits - 2)
                        description += "; ";
                }
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

        private string AddDetails(string trait, bool start)
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
                default:
                    return "";
            }
        }
    }
