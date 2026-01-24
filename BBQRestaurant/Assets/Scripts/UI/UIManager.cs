using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public ObjectPool<CustomUI> CustomerUIPool;//¹Ë¿Íui
    public ObjectPool<IngredientUI> IngredientUIPool;//Ê³²ÄUI

    private void Awake()
    {
        Instance = this;
    }


}
