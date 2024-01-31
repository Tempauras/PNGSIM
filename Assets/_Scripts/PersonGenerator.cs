using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PersonGenerator : MonoBehaviour
{
    [SerializeField] private List<Trait> _traitListToGenerateFrom = new List<Trait>();
    [SerializeField, Tooltip("x is min inclusive, y is max exclusive")] private Vector2 _numberOfTraitsRange;
    [SerializeField] private List<string> _nameListToGenerateFrom = new List<string>();
    [SerializeField, Tooltip("x is min inclusive, y is max exclusive")] private Vector2 _ageRange;
    [SerializeField, Tooltip("x is min inclusive, y is max exclusive")] private Vector2 _numberOfPersonToGenerateRange;

    public List<Person> Persons = new List<Person>();
    // Start is called before the first frame update
    void Start()
    {
        int numberOfPerson = Random.Range((int) _numberOfPersonToGenerateRange.x, (int) _numberOfPersonToGenerateRange.y);
        for (int i = 0; i < numberOfPerson; i++)
        {
            Persons.Add(GeneratePerson());
            Debug.Log($"Generated new Person number {i + 1}, Name: {Persons[i].Name}, Age: {Persons[i].Age} with {Persons[i].Traits.Count} traits");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Person GeneratePerson()
    {
        int age = Random.Range((int)_ageRange.x, (int)_ageRange.y);
        int numberOfTraits = Random.Range((int) _numberOfTraitsRange.x, (int) _numberOfTraitsRange.y);
        List<Trait> personTraits = new List<Trait>();
        for (int i = 0; i < numberOfTraits; i++)
        {
            PickTrait(ref personTraits);
        }
        string name = _nameListToGenerateFrom[Random.Range(0, _nameListToGenerateFrom.Count)];

        Person person = new Person(name, age, personTraits);
        return person;
    }

    private void PickTrait(ref List<Trait> CurrentTraits)
    {
        bool hasSelectedNewCorrectTraits = false;

        while(!hasSelectedNewCorrectTraits)
        {
            Trait newTrait = _traitListToGenerateFrom[Random.Range(0, _traitListToGenerateFrom.Count)];
            if (!CurrentTraits.Find(x => x == newTrait) && !CurrentTraits.Exists(x => x.NonCompatibleTrait.Exists(x => x == newTrait)))
            {
                Debug.Log("Adding " + newTrait.name + " to current traits.");
                CurrentTraits.Add(newTrait);
                hasSelectedNewCorrectTraits = true;
            }
        }
    }
}
