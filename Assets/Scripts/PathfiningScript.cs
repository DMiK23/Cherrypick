using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathfiningScript : MonoBehaviour //class that controls pathfinding
{
    private GameObject[,] _board;
    [SerializeField] BoardGeneratorScript _boardManager;
    private FieldInfoScript _startField;
    private FieldInfoScript _endField;
    [SerializeField] Text _pathText;

    public void FindPath(int n)//clears board and uses chosen algorithm
    {
        _board = _boardManager.GetBoard();
        ClearPaths();
        switch (n)
        {
            case 1:
                FindPathQueue();
                print("Finding with queue");
                break;
            case 2:
                FindPathRecursion(0, _startField);
                print("Finding with Recursion");
                break;
            default:
                print("No algorithm found");
                break;
        }
        ColorPath();
    }

    void ClearPaths()//resets all fields' wieghts and predecessors to default
    {
        foreach (GameObject field in _board)
        {
            FieldInfoScript fis = field.GetComponent<FieldInfoScript>();
            if (fis._fieldType == 2)
            {
                fis._pathWeight = 0;
                _startField = fis;
            }
            else
            {
                fis._pathWeight = int.MaxValue;
                fis._previousField = null;
                if (fis._fieldType == 3)
                    _endField = fis;
            }            
        }
    }

    bool FindPathQueue()//finds path with queue
    {
        Queue<FieldInfoScript> queue = new Queue<FieldInfoScript>();//queue of fields (starts with start)
        queue.Enqueue(_startField);
        Queue<int> weightQueue = new Queue<int>();//queue of corresponding weights (starts with 0)
        weightQueue.Enqueue(0);
        while (queue.Count > 0)
        {
            FieldInfoScript field = queue.Dequeue();
            int tWeight = weightQueue.Dequeue();
            Vector3 neighbourPos;
            for (int i = -1; i < 2; i=i+2)//distributes weight for X-axis neighbours
            {
                neighbourPos = field.gameObject.transform.position + new Vector3(i, 0, 0);
                try
                {
                    FieldInfoScript fis = _board[(int)neighbourPos.x, (int)neighbourPos.z].GetComponent<FieldInfoScript>();
                    if (fis._pathWeight > tWeight + 1 && fis._fieldType != 1)//if current field weight has more than the wight in the new current path AND isn't a wall
                    {
                        fis._pathWeight = tWeight + 1;//assign new weight
                        fis._previousField = field;//assign new predecessor
                        if (fis._fieldType == 3) return true;//function ends if end field is found 
                        queue.Enqueue(fis);//enqueues current field's neighbour
                        weightQueue.Enqueue(tWeight + 1);
                    }
                }
                catch (System.IndexOutOfRangeException)//catches if the neighbour would be off the grid
                {

                }
            }
            for (int j = -1; j < 2; j = j + 2)//distributes weight for Z-axis neighbours
            {
                neighbourPos = field.gameObject.transform.position + new Vector3(0, 0, j);
                try
                {
                    FieldInfoScript fis = _board[(int)neighbourPos.x, (int)neighbourPos.z].GetComponent<FieldInfoScript>();
                    if (fis._pathWeight > tWeight + 1 && fis._fieldType != 1)
                    {
                        fis._pathWeight = tWeight + 1;
                        fis._previousField = field;
                        if (fis._fieldType == 3) return true;
                        queue.Enqueue(fis);
                        weightQueue.Enqueue(tWeight + 1);
                    }
                }
                catch (System.IndexOutOfRangeException)
                {

                }
            }
        }
        return false;//if end file is not found
    }

    //NOTE FindPathRecursion function is slower than FindPathQueue, becouse it has to check all possible paths
    bool FindPathRecursion(int tWeight, FieldInfoScript field)//finds path with recursion, must be first called with 0 and start field attributes
    {
        
        Vector3 neighbourPos;
        for (int i = -1; i < 2; i = i + 2) //distributes weight for X-axis neighbours
        {
            neighbourPos = field.gameObject.transform.position + new Vector3(i, 0, 0);
            try
            {
                FieldInfoScript fis = _board[(int)neighbourPos.x, (int)neighbourPos.z].GetComponent<FieldInfoScript>();
                if (fis._pathWeight > tWeight + 1 && fis._fieldType != 1)//if current field weight has more than the wight in the new current path AND isn't a wall
                {
                    fis._pathWeight = tWeight + 1;//assign new weight
                    fis._previousField = field;//assign new predecessor
                    if (fis._fieldType == 3) return true; //function ends if end field is found 
                    FindPathRecursion(tWeight + 1, fis);//calls function for current field's neighbour
                }
            }
            catch (System.IndexOutOfRangeException)//catches if the neighbour would be off the grid
            {

            }
        }
        for (int j = -1; j < 2; j = j + 2)//distributes weight for Z-axis neighbours
        {
            neighbourPos = field.gameObject.transform.position + new Vector3(0, 0, j);
            try
            {
                FieldInfoScript fis = _board[(int)neighbourPos.x, (int)neighbourPos.z].GetComponent<FieldInfoScript>();
                if (fis._pathWeight > tWeight + 1 && fis._fieldType != 1) 
                {
                    fis._pathWeight = tWeight + 1;
                    fis._previousField = field;
                    if (fis._fieldType == 3) return true;
                    FindPathRecursion(tWeight + 1, fis);
                }
            }
            catch (System.IndexOutOfRangeException)
            {

            }
        }
        return false;//if end file is not found
    }

    void ColorPath() //colors the shortest path, shows distance and whether if the path is impossible 
    {
        try
        {
            FieldInfoScript currenField = _endField._previousField; //starts with end field
            StartCoroutine(OutputText("Path distance: " + _endField._pathWeight, 5));
            while (currenField._pathWeight > 0) //colors the path up to the start
            {
                currenField.SetAsPath();
                currenField = currenField._previousField;
            }
        }
        catch (System.NullReferenceException)
        {
            StartCoroutine(OutputText("No path possible", 5));
            print("No path possible");
        }
        
        
    }

    IEnumerator OutputText(string s, int n) //Shows given text s fot n seconds
    {
        _pathText.text = s;
        yield return new WaitForSeconds(n);
        _pathText.text = "";
    }
}
