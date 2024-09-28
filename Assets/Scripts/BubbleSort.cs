using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using TMPro;

public class BubbleSort : MonoBehaviour
{
    float[] array;
    List<GameObject> mainObjects;
    public GameObject prefab;

    private Thread _thread;

    private bool arrayChanged = false;

    private readonly object _lock = new object();

    [SerializeField] private bool isLock = false;
    public enum SortingAlgorithm
    {
        BubbleSort,
        QuickSort
    }

    [SerializeField] private SortingAlgorithm selectedAlgorithm = SortingAlgorithm.BubbleSort;


    [SerializeField] private TMP_Dropdown algorithmDropdown;
    [SerializeField] private Toggle lockToggle;
    [SerializeField] private Button startButton;


    void Start()
    {

        mainObjects = new List<GameObject>();
        array = new float[30000];
        for (int i = 0; i < 30000; i++)
        {
            array[i] = (float)Random.Range(0, 1000)/100;
        }

        startButton.onClick.AddListener(StartSorting);

    }

    void ResetArrayAndObjects()
    {
        
        if (_thread != null && _thread.IsAlive)
        {
            _thread.Abort();
        }

        arrayChanged = true;

        
        array = new float[30000];
        for (int i = 0; i < 30000; i++)
        {
            array[i] = Random.Range(0f, 10f);
        }

        
        foreach (GameObject obj in mainObjects)
        {
            Destroy(obj);
        }
        mainObjects.Clear();



        //TO DO 4
        //Call the three previous functions in order to set up the exercise

        logArray();

        spawnObjs();

        
        updateHeights();
    }


    void StartSorting()
    {
        
        int algorithmIndex = algorithmDropdown.value;
        selectedAlgorithm = (SortingAlgorithm)algorithmIndex;

        
        isLock = lockToggle.isOn;

        
        ResetArrayAndObjects();


        //TO DO 5
        //Create a new thread using the function "bubbleSort" and start it.

        if (selectedAlgorithm == SortingAlgorithm.BubbleSort)
        {
            if (isLock)
            {
                _thread = new Thread(bubbleSortWithLock);
            }
            else
            {
                _thread = new Thread(bubbleSort);
            }
        }
        else if (selectedAlgorithm == SortingAlgorithm.QuickSort)
        {

            _thread = new Thread(QuickSortThread);

        }
        _thread.Start();
    }



    void Update()
    {
        //TO DO 6
        //Call ChangeHeights() in order to update our object list.
        //Since we'll be calling UnityEngine functions to retrieve and change some data,
        //we can't call this function inside a Thread
        if (arrayChanged)
        {
            updateHeights();
        }




    }

    //TO DO 5
    //Create a new thread using the function "bubbleSort" and start it.
    void bubbleSort()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        int i, j;
        int n = array.Length;
        bool swapped;
        for (i = 0; i < n- 1; i++)
        {
            swapped = false;
            for (j = 0; j < n - i - 1; j++)
            {
                if (array[j] > array[j + 1])
                {
                    (array[j], array[j+1]) = (array[j+1], array[j]);
                    swapped = true;
                }
            }
            if (swapped == false)
                break;
        }
        //You may debug log your Array here in case you want to. It will only be called one the bubble algorithm has finished sorting the array
        stopwatch.Stop();
        Debug.Log("BubbleSort completado en " + stopwatch.ElapsedMilliseconds + " ms");
        Debug.Log("array after being bubble ssorted: ");
        logArray();

        
    }

    void bubbleSortWithLock()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        int i, j;
        int n = array.Length;
        bool swapped;
        for (i = 0; i < n - 1; i++)
        {
            swapped = false;
            for (j = 0; j < n - i - 1; j++)
            {
                lock (_lock)
                {
                    if (array[j] > array[j + 1])
                    {
                        (array[j], array[j + 1]) = (array[j + 1], array[j]);
                        swapped = true;
                    }
                }
            }
            if (swapped == false)
                break;
        }
        stopwatch.Stop();
        Debug.Log("BubbleSort con Lock completado en " + stopwatch.ElapsedMilliseconds + " ms");
        Debug.Log("Array ordenado: " + string.Join(" ", array));
        logArray();

        
    }


    void logArray()
    {
        string text = "";

        //TO DO 1
        //Simply show in the console what's inside our array.

        text = "Array contents: " + string.Join(" ", array);

        Debug.Log(text);
    }
    
    void spawnObjs()
    {
        //TO DO 2
        //We should be storing our objects in a list so we can access them later on.

        for (int i = 0; i < array.Length; i++)
        {
            //We have to separate the objs accordingly to their width, in which case we divide their position by 1000.
            //If you decide to make your objs wider, don't forget to up this value

            mainObjects.Add(Instantiate(prefab, new Vector3((float)i / 1000, 
                this.gameObject.GetComponent<Transform>().position.y, 0), Quaternion.identity));
        }

    }

    //TO DO 3
    //We'll just change the height of every obj in our list to match the values of the array.
    //To avoid calling this function once everything is sorted, keep track of new changes to the list.
    //If there weren't, you might as well stop calling this function

    bool updateHeights()
    {

        bool changed = false;
        for (int i = 0; i < array.Length; i++)
        {
            Vector3 newScale = mainObjects[i].transform.localScale;

            
            if (newScale.y != array[i])
            {
                
                newScale.y = array[i];
                mainObjects[i].transform.localScale = newScale;

                
                changed = true;
            }
        }
        return changed;
    }

    void OnApplicationQuit()
    {
        
        if (_thread != null && _thread.IsAlive)
        {
            _thread.Abort();
        }
    }


    void QuickSort(float[] array, int left, int right)
    {
        int i = left, j = right;
        float pivot = array[(left + right) / 2];

        while (i <= j)
        {
            while (array[i] < pivot)
                i++;
            while (array[j] > pivot)
                j--;

            if (i <= j)
            {
                float temp = array[i];
                array[i] = array[j];
                array[j] = temp;
                i++;
                j--;
            }
        }

        if (left < j)
            QuickSort(array, left, j);
        if (i < right)
            QuickSort(array, i, right);



    }


    void QuickSortThread()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        if (isLock)
        {
            QuickSortWithLock(array, 0, array.Length - 1);

        }
        else
        {
            QuickSort(array, 0, array.Length - 1);

        }

        stopwatch.Stop();


        if (isLock)
        {
            Debug.Log("QuickSort completado en " + stopwatch.ElapsedMilliseconds + " ms");


        }
        else
        {
            Debug.Log("QuickSort con Lock completado en " + stopwatch.ElapsedMilliseconds + " ms");

        }

        Debug.Log("Array de ser quicksorted:");
        logArray();
    }



    void QuickSortWithLock(float[] array, int left, int right)
    {
        

        int i = left, j = right;
        float pivot = array[(left + right) / 2];

        while (i <= j)
        {
            while (array[i] < pivot)
                i++;
            while (array[j] > pivot)
                j--;

            if (i <= j)
            {
                lock (_lock)
                {
                    float temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                    arrayChanged = true;
                }
                i++;
                j--;
            }
        }

        if (left < j)
            QuickSort(array, left, j);
        if (i < right)
            QuickSort(array, i, right);

        

        
    }



}
