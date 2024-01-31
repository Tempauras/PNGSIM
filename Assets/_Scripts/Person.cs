using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    [SerializeField] private string _name = "";
    [SerializeField] private int _age = 18;
    [SerializeField] private List<Trait> _traits = new List<Trait>();
    [SerializeField] private Sprite _sprite;

    public string Description = "";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
