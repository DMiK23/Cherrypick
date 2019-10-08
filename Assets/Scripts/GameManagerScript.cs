using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameManagerScript : MonoBehaviour //class that manages basic game functionality
{
    [SerializeField] BoardGeneratorScript _boardGenerator;
    [SerializeField] PathfiningScript _pathfiningScript;
    [SerializeField] Button load1;
    [SerializeField] Button load2;
    [SerializeField] Button load3;
    [SerializeField] Button load4;

    void Start()
    {
        _boardGenerator.CreateMap();
        ShowWhichLoad();
    }

    void ShowWhichLoad() //Activates buttons if there are savefiles for them
    {
        Button[] buttonArr = new Button[] { load1, load2, load3, load4 };
        for (int i = 0; i < buttonArr.Length; i++)
        {
            try
            {

                StreamReader sr = new StreamReader(Path.Combine(Application.persistentDataPath, "map" + (i + 1) + ".txt"));
                buttonArr[i].interactable = true;
            }
            catch (FileNotFoundException)
            {
            }
        }
    }

    public void Exit() //Quits application
    {
        Application.Quit();
    }
    

}
