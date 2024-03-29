using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private PersonGenerator personGenerator;
    
    [SerializeField] private RectTransform TraitRoot;
    [SerializeField] private Button addButton;
    [SerializeField] private TMP_Dropdown addDropdown;
    [SerializeField] private Button regenButton;
    [SerializeField] private Button[] personButtons;
    [SerializeField] private Button exitButton;

    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;

    [SerializeField] private GameObject traitPrefab;

    private Person _currentPerson;

    private void Start()
    {
        regenButton.onClick.AddListener(()=>
        {
            int personIndex = personGenerator.Persons.IndexOf(_currentPerson);
            personGenerator.GeneratePersonOnClick();
            RefreshButtons();
            SelectPerson(personIndex);
        });
        regenButton.onClick.AddListener(RefreshButtons);
        
        addButton.onClick.AddListener(AddTrait);

        exitButton.onClick.AddListener(ExitGame);

        personGenerator.GeneratePersonOnClick();
        RefreshButtons();
        
        //personGenerator.FaceController.initFinished.AddListener(delegate { SelectPerson(0); });
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForEndOfFrame();
        SelectPerson(0);
    }

    private void RefreshButtons()
    {
        for (int i = 0; i < personButtons.Length; i++)
        {
            var j = i;
            personButtons[j].onClick.RemoveAllListeners();
            personButtons[j].onClick.AddListener(delegate { SelectPerson(j);});

            personButtons[j].GetComponentInChildren<TextMeshProUGUI>().text = personGenerator.Persons[j].Name;
        }
    }
    
    private void SelectPerson(int index)
    {
        foreach (var butt in personButtons)
        {
            butt.interactable = true;
        }
        personButtons[index].interactable = false;
        _currentPerson = personGenerator.Persons[index];
        foreach(Transform child in TraitRoot.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < _currentPerson.Traits.Count; i++)
        {
            GameObject traitGameObject = Instantiate(traitPrefab, TraitRoot);
            traitGameObject.GetComponentInChildren<TextMeshProUGUI>().text = _currentPerson.Traits[i].TraitName;
            
            Trait trait = _currentPerson.Traits[i];
            traitGameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate{ DeleteTrait(index, trait);});
            
        }
        
        UpdateDropDown();
        
        personGenerator.FeedFaceController(_currentPerson);

        _textMeshProUGUI.text = _currentPerson.Description;
    }

    private void DeleteTrait(int index, Trait trait)
    {
        if (personGenerator.Persons[index].Traits.Count <= 1)
        {
            return;
        }
        personGenerator.Persons[index].RemoveTrait(trait);
        
        SelectPerson(index);
    }

    private void UpdateDropDown()
    {
        addDropdown.options.Clear();
        List<Trait> traitsList = new List<Trait>();
        traitsList.AddRange(personGenerator.TraitListToGenerateFrom);
    
        traitsList.RemoveAll(x => _currentPerson.Traits.Contains(x) || _currentPerson.GetAllTraitIncompatibleWithCurrentTraits().Contains(x));
    
        foreach (Trait trait in traitsList)
        {
            addDropdown.options.Add(new TMP_Dropdown.OptionData(trait.TraitName));
        }

        if (addDropdown.options.Count > 0)
        {
            addDropdown.captionText.text = addDropdown.options[0].text;
        }
        else
        {
            addDropdown.captionText.text = "None left";
        }
    }
    
    private void AddTrait()
    {
        int optionId = addDropdown.value;
        TMP_Dropdown.OptionData data = addDropdown.options[optionId];
        
        _currentPerson.AddTrait(personGenerator.TraitListToGenerateFrom.First(x =>x.TraitName == data.text));

        SelectPerson(personGenerator.Persons.IndexOf(_currentPerson));
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
