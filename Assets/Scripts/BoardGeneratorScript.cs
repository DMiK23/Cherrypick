using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class BoardGeneratorScript : MonoBehaviour //class that manages the board
{
    [SerializeField] int _boardSize = 10;
    [SerializeField] int _numberOfObstacles = 15;
    [SerializeField] GameObject _fieldPrefab;
    private GameObject[,] _board;
    [SerializeField] Slider _sizeSlider;


 

    public void CreateMap()//generates map with current settings and sets the camera to show the whole board
    {
        ClearBoard();
        GenerateBoard(_boardSize);
        GeneratePOI();
        GenerateObstacles(_numberOfObstacles);
        GameObject camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
        camera.transform.SetPositionAndRotation(new Vector3((_boardSize/2),_boardSize, (_boardSize / 2) - 0.5f),camera.transform.rotation);
    }

    private void GenerateBoard(int n)//generates the board usinf the field prefab
    {
        _board = new GameObject[n, n];
        for (int i = 0; i < n; i++){
            for (int j = 0; j < n; j++){
                GameObject field = Instantiate(_fieldPrefab, new Vector3(i, 0, j), Quaternion.identity);
                _board[i, j] = field;
                field.transform.parent = this.gameObject.transform; 
            }
        }
    }

    private void GeneratePOI() //genarates start and end fields
    {
        FieldInfoScript _start = _board[UnityEngine.Random.Range(0, _boardSize), UnityEngine.Random.Range(0, _boardSize)].GetComponent<FieldInfoScript>();
        _start.SetAsStart();

        FieldInfoScript end = _board[UnityEngine.Random.Range(0, _boardSize), UnityEngine.Random.Range(0, _boardSize)].GetComponent<FieldInfoScript>(); ;
        while (end == _start)
            end = _board[UnityEngine.Random.Range(0, _boardSize), UnityEngine.Random.Range(0, _boardSize)].GetComponent<FieldInfoScript>();
        end.SetAsEnd();
    }

    private void GenerateObstacles (int m)//genarates obstacle fields
    {
        int freeFields = _boardSize * _boardSize - 2;
        if (m >= freeFields)//if there is more obstacle than free fields
        {
            foreach (GameObject field in _board)
                field.GetComponent<FieldInfoScript>().SetAsObstacle();
            return;
        }

        int approximateQuantityOfEachObstacle = (m - m % 4) / 4;
        int obstaclesLeft = m;

        for (int i = 0; i < (int)approximateQuantityOfEachObstacle/4; i++) //tries to place a quater of obstacle fields as 2*2 obstacles
        {
            int a = UnityEngine.Random.Range(0, _boardSize - 1);
            int b = UnityEngine.Random.Range(0, _boardSize - 1);
            if (_board[a ,b].GetComponent<FieldInfoScript>()._fieldType == 0 &&
                _board[a+1, b].GetComponent<FieldInfoScript>()._fieldType == 0 &&
                _board[a, b+1].GetComponent<FieldInfoScript>()._fieldType == 0 &&
                _board[a+1, b+1].GetComponent<FieldInfoScript>()._fieldType == 0)
            {
                _board[a, b].GetComponent<FieldInfoScript>().SetAsObstacle();
                _board[a + 1, b].GetComponent<FieldInfoScript>().SetAsObstacle();
                _board[a, b + 1].GetComponent<FieldInfoScript>().SetAsObstacle();
                _board[a + 1, b + 1].GetComponent<FieldInfoScript>().SetAsObstacle();
                obstaclesLeft = obstaclesLeft - 4;
                
            }
        }

        for (int i = 0; i < (int)approximateQuantityOfEachObstacle/2; i++)//tries to place a quater of obstacle fields as 2*1 obstacles
        {
            int a = UnityEngine.Random.Range(0, _boardSize - 1);
            int b = UnityEngine.Random.Range(0, _boardSize);
            if (_board[a, b].GetComponent<FieldInfoScript>()._fieldType == 0 &&
                _board[a + 1, b].GetComponent<FieldInfoScript>()._fieldType == 0)
            {
                _board[a, b].GetComponent<FieldInfoScript>().SetAsObstacle();
                _board[a + 1, b].GetComponent<FieldInfoScript>().SetAsObstacle();
                obstaclesLeft = obstaclesLeft - 2;
            }
        }

        for (int i = 0; i < (int)approximateQuantityOfEachObstacle/2; i++)//tries to place a quater of obstacle fields as 1*2 obstacles
        {
            int a = UnityEngine.Random.Range(0, _boardSize);
            int b = UnityEngine.Random.Range(0, _boardSize - 1);
            if (_board[a, b].GetComponent<FieldInfoScript>()._fieldType == 0 &&
                _board[a, b+1].GetComponent<FieldInfoScript>()._fieldType == 0)
            {
                _board[a, b].GetComponent<FieldInfoScript>().SetAsObstacle();
                _board[a, b+1].GetComponent<FieldInfoScript>().SetAsObstacle();
                obstaclesLeft = obstaclesLeft - 2;
            }
        }

        while (obstaclesLeft > 0)//places the rest of obstacle fields as 1*1 obstacles
        {
            int a = UnityEngine.Random.Range(0, _boardSize);
            int b = UnityEngine.Random.Range(0, _boardSize - 1);
            if (_board[a, b].GetComponent<FieldInfoScript>()._fieldType == 0)
            {
                _board[a, b].GetComponent<FieldInfoScript>().SetAsObstacle();
                obstaclesLeft--;
            }
        }
    }

    public void SaveMap(int n)//saves map to .txt file with an 'n' index
    {
        byte[,] map = new byte[_boardSize, _boardSize];
        for (int i = 0; i < _boardSize; i++)
        {
            for (int j = 0; j < _boardSize; j++)
            {
                map[i, j] = _board[i, j].GetComponent<FieldInfoScript>()._fieldType;
            }
        }
        StreamWriter sw = new StreamWriter(Path.Combine(Application.persistentDataPath, "map" + n + ".txt"));
        sw.WriteLine(_boardSize);//in the first line size of the board is saved
        foreach (byte t in map)//in the second line all of the field types are saved in order
        {
            sw.Write(t + " ");
            
        }
        sw.Close();
    }

    public void LoadMap(int n)//attempts to load map from .txt file with an 'n' index and if successfull, generates loaded map
    {
        string mapSeed;
        try //attempts to load map seed
        {
            StreamReader sr = new StreamReader(Path.Combine(Application.persistentDataPath, "map" + n + ".txt"));
            _boardSize = Int32.Parse(sr.ReadLine());
            _sizeSlider.value = _boardSize; //updates the slider value
            mapSeed = sr.ReadLine();
        }
        catch (FileNotFoundException)
        {

            print("file not found");
            return;
        }

        ClearBoard(); //clears previos board

        _board = new GameObject[_boardSize, _boardSize];
        int fieldNumber = 0;
        for (int i = 0; i < _boardSize; i++)//generates each field
        {
            for (int j = 0; j < _boardSize; j++)
            {
                GameObject field = Instantiate(_fieldPrefab, new Vector3(i, 0, j), Quaternion.identity);
                _board[i, j] = field;
                field.transform.parent = this.gameObject.transform;
                string[] strArr = mapSeed.Split(' ');
                switch (Int32.Parse(strArr[fieldNumber]))
                {
                    case 1:
                        field.GetComponent<FieldInfoScript>().SetAsObstacle();                        
                        break;
                    case 2:
                        field.GetComponent<FieldInfoScript>().SetAsStart();
                        break;
                    case 3:
                        field.GetComponent<FieldInfoScript>().SetAsEnd();
                        break;
                    default:
                        break;
                }
                fieldNumber++;
            }
        }
        GameObject camera = GameObject.FindGameObjectsWithTag("MainCamera")[0]; //sets the camera to show the whole board
        camera.transform.SetPositionAndRotation(new Vector3((_boardSize / 2), _boardSize, (_boardSize / 2)-0.5f), camera.transform.rotation);
        return;
    }

    public GameObject[,] GetBoard() //returns the board
    {
        return _board;
    }

    public void ClearBoard() //destroys all fields in current board
    {
        foreach (Transform childT in this.gameObject.GetComponentsInChildren<Transform>())
        {
            if (childT != this.gameObject.transform) GameObject.Destroy(childT.gameObject);
        }
    }

    public void SetSize(Slider slider)//when slider value is changed, updates the relevant variable and text above slider
    {
        _boardSize = (int)slider.value;
        slider.gameObject.GetComponentInChildren<Text>().text = "Size - " + _boardSize.ToString();
    }
    public void SetNumberOfObstacles(Slider slider)//when slider value is changed, updates the relevant variable and text above slider
    {
        _numberOfObstacles = (int)slider.value;
        slider.gameObject.GetComponentInChildren<Text>().text = "Obstacles - " + _numberOfObstacles.ToString();
    }
}
