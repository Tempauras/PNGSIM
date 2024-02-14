using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private GameObject traitPrefab;

    private void Start()
    {
        regenButton.onClick.AddListener(personGenerator.GeneratePersonOnClick);
        regenButton.onClick.AddListener(RefreshButtons);

        personGenerator.GeneratePersonOnClick();
        RefreshButtons();
        
        SelectPerson(0, personButtons[0]);
        
    }

    private void RefreshButtons()
    {
        for (int i = 0; i < personButtons.Length; i++)
        {
            var j = i;
            personButtons[j].onClick.RemoveAllListeners();
            personButtons[j].onClick.AddListener(delegate { SelectPerson(j, personButtons[j]);});

            personButtons[j].GetComponentInChildren<TextMeshProUGUI>().text = personGenerator.Persons[j].Name;

        }
    }
    
    private void SelectPerson(int index, Button button)
    {
        foreach (var butt in personButtons)
        {
            butt.interactable = true;
        }
        button.interactable = false;
        Person person = personGenerator.Persons[index];
        foreach(Transform child in TraitRoot.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < person.Traits.Count; i++)
        {
            GameObject traitGameObject = Instantiate(traitPrefab, TraitRoot);
            traitGameObject.GetComponentInChildren<TextMeshProUGUI>().text = person.Traits[i].TraitName;
            
            Trait trait = person.Traits[i];
            traitGameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate{ DeleteTrait(index, trait);});
            
        }
        
        personGenerator.FeedFaceController(person);
    }

    private void DeleteTrait(int index, Trait trait)
    {
        personGenerator.Persons[index].RemoveTrait(trait);
        
        SelectPerson(index, personButtons[index]);
    }

    private void UpdateDropDown()
    {
        
    }
    
    private void AddTrait(int index, Trait trait)
    {
        
        
        
        
    }
    
    
    
    
}
