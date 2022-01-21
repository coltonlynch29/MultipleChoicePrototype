using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GlobalUtils;
using TMPro;

public class MultipleChoice : MonoBehaviour
{
    [Header("Player controller")]
    private FPS_Controller player;

    [Header("MultipleChoiceBox Strings")] 
    private string question;
    private string response;

    [Header("MultipleChoiceBox Objs")]    
    public TextMeshProUGUI questionDisplay;
    public GameObject exitButton;

    [Header("Button Content List")]
    public List<Button> buttonContent;

    [Header("New MultipleChoiceBox Objs")]
    public Image instructorImage;
    private string MultipleChoiceListJsonPath = "JSON/MultipleChoice_JSON/";
    public string MultipleChoiceListJsonFile;
    public int questionIndex;

    public class MultiplechoiceButtonData
    {
        public string content;
        public bool isCorrectAnswer = false;
    }


    // test list
    public List<MultiplechoiceButtonData> choicelist = new List<MultiplechoiceButtonData>();

    //JSON serialized variables
    [System.Serializable]
    public class MultiplechoiceButtonPanelData
    {
        public MultiplechoiceButtonlQuestionsData[] Questions;
    }

    [System.Serializable]
    public class MultiplechoiceButtonlQuestionsData
    {
        public string[] Choices;
        public string MainQuestion;
        public string Response;
        public int CorrectChoice;
    }

    private void Awake()
    {
       
        //Store the Player Controller
        if (FindObjectOfType<FPS_Controller>())
        {
            player = FindObjectOfType<FPS_Controller>();
        }

    }

    private void OnEnable()
    {

        //Generate Radio Panel Questions        
        GenerateRadioPanelButtons();
        
    }

    private void OnDisable()
    {
        // Reset Radio Panel to Default
        ResetRadioPanel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GenerateRadioPanelButtons()
    {
        //Clear ChoiceList
        choicelist.Clear();

        //Store Questions JSON File
        TextAsset RadioPanelJson = Resources.Load<TextAsset>(radioPanelListJsonPath + radioPanelListJsonFile);

        //Check for the file existing...
        if (RadioPanelJson != null)
        {
            // Print information to list to check
            RadioPanelData radJSONData = new RadioPanelData();

            // Parse Json Data into RadioPanelData
            radJSONData = JsonUtility.FromJson<RadioPanelData>(RadioPanelJson.text);
            
            //Grab the question based on the current index
            RadioPanelQuestionsData questionJSON = radJSONData.Questions[questionIndex];

            // Loop Through Choices in Json
            for (int i = 0; i < questionJSON.Choices.Length; i++)
            {
                RadioButtonData newQuestion = new RadioButtonData();

                if (i == questionJSON.CorrectChoice)
                {
                    newQuestion.isCorrectAnswer = true;
                }

                // Set New Question Content
                newQuestion.content = questionJSON.Choices[i];
                
                // Set Question
                question = questionJSON.MainQuestion;

                //Set Question Display
                questionDisplay.text = question;

                // Set Response
                response = questionJSON.Response;

                // Check if the Content is Empty
                if (!string.IsNullOrEmpty(newQuestion.content))
                {
                    // If not Empty then add to Choice List 
                    choicelist.Add(newQuestion);
                }
                else
                {
                    //Turn off Button if entry is null or empty
                    buttonContent[i].gameObject.SetActive(false);
                }

            }

            // Shuffle List of Choices
            choicelist.Shuffle();


            // Set Button Text to Entrys in Choice List
            for (int i = 0; i < choicelist.Count; i++)
            {
                buttonContent[i].GetComponentInChildren<TextMeshProUGUI>().text = choicelist[i].content;

                //Reset highlight component
                buttonContent[i].GetComponent<UnityEngine.UI.Outline>().enabled = false;

               
                // Check if Choice in the list is the Correct Answer
                if (choicelist[i].isCorrectAnswer)
                {
                    // Highlight the Button of the Correct Answer
                    buttonContent[i].GetComponent<UnityEngine.UI.Outline>().enabled = true;
                }
                
            }

        }

    }


    //Close Panel
    public void ExitQuestionPanel()
    {
        if (this.gameObject.activeInHierarchy == true)
        {
            this.gameObject.SetActive(false);
            player.enabled = true;
            exitButton.SetActive(false);

        }
    }

    //Reset Radio panel
    public void ResetRadioPanel()
    {        
        // Clear buttons and reactivate them
        foreach (Button radiobutton in buttonContent)
        {
            if (radiobutton.gameObject.activeInHierarchy == false)
            {
                radiobutton.gameObject.SetActive(true);
                radiobutton.gameObject.GetComponent<UnityEngine.UI.Outline>().enabled = false;
                radiobutton.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }

        }

        // Clear Choicelist
        choicelist.Clear();

    }

    // check the answer when the user makes a selection
    public void CheckChoice( int ChoiceNum)
    {    
        // Check if the Selected choice is the correct answer
        if (choicelist[ChoiceNum].isCorrectAnswer)
        {
            // Push CompleteTask to Scene Machine
            manager.CompleteTask(manager.currentTask);

            // Set Response
            questionDisplay.text = response;

            // Deactivate Choice Buttons
            foreach(Button radiobutton in buttonContent)
            {
               
                radiobutton.gameObject.SetActive(false);

            }

            // Set the exit button to be active             
            {
                exitButton.SetActive(true);
            }
        }

        else
        {
            // Push Completed Wrong to scene machine
            Debug.log("You have selected the wrong answer");
        }
    }



}
