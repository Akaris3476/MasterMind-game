
using System.Runtime.CompilerServices;

static class GameSize
{
    public static int NumberOfTurns = 6;
    public static int NumberOfCodes = 4;
}

static class Colors //color list
{
    private static readonly List<string> _ColorList = ["red", "green", "blue", "grey", "orange", "brown"];
    public static List<string> ColorList { get { return _ColorList; } }
    
}

class Turn
{

    private List<string> _TurnColors = new();
    public List<string> TurnColors { get { return _TurnColors; } }

    private List<string> _TurnMatches = new();
    public List<string> TurnMatches { get { return _TurnMatches; } set { _TurnMatches = value; } }
    
    
    public Turn(string[] playerInput)
    {
        foreach (string color in playerInput)
        {
            
            if (Colors.ColorList.Contains(color)) //input check
            {
                _TurnColors.Add(color);
            }  else {
                throw new ArgumentException("Invalid colors. Try again.\n");
            }
        }
    }

}


class Game
{

    private List<string> _EnemyDeck = [];          //list with enemy codes

    public List<string> EnemyDeck
    {
        get { return _EnemyDeck; }
    }
    
    private List<Turn> _TurnsHistory = new();
    
    public Turn TurnsHistory
    { 
        set { _TurnsHistory.Add(value);}
        get { return _TurnsHistory.Last(); }
    }

    
    

    
    public static void RandomColorFill(List<string> list, List<string> sample, int size)
    {
        int sampleMaxIndex = sample.Count;
        
        Random rnd = new Random();
        

        for (int i = 0; i < size; i++)
        {
            int j = rnd.Next(0,sampleMaxIndex);
            list.Add(sample[j]) ;

        }
        
    }

    
    public void MakeTurn()
    { 
        
        Turn turn = null;
        while (turn ==  null)
        {
            try
            {
                string[] playerInput = Console.ReadLine().Split(' '); //player inputs colors
                
                if (playerInput.Length != GameSize.NumberOfCodes)
                {
                    throw new ArgumentException("Invalid number of colors. Try again.\n");
                }
                
                turn = new Turn(playerInput);   //create new turn and send turn colors
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            TurnsHistory = turn; //add turn to game history
        }
    }
    
    public void MatchCheck()
    {   
        //check and write matches
        Turn currentTurn = TurnsHistory;
        List<string> tempEnemyDeck = EnemyDeck.ToList();
        List<string> playerColors = currentTurn.TurnColors.ToList(); //extracting player colors from turn
        
        //firstly check for black matches to prevent excessive whites
        for (int k = 0; k < GameSize.NumberOfCodes; k++) 
        {

            if (playerColors[k] == tempEnemyDeck[k]) 
            { 
                currentTurn.TurnMatches.Add("black"); 
                tempEnemyDeck[k] = null; //delete enemy color 
                playerColors[k] = null; //to prevent false whites
            }
        }
        
        //check for white matches
        for (int i = 0; i < GameSize.NumberOfCodes; i++) 
        {
            if (tempEnemyDeck.Contains(playerColors[i]) && playerColors[i] != null) //check color match
            {
                currentTurn.TurnMatches.Add("white"); //white for color match
                tempEnemyDeck[tempEnemyDeck.IndexOf(playerColors[i])] = null; //to prevent repetitions
            }
        }
        
        //write matches
        foreach (string color in currentTurn.TurnMatches)
        {
            if (color == "black")
            {
                Console.WriteLine(color);
            }
        }
        foreach (string color in currentTurn.TurnMatches)
        {
            if (color == "white")
                Console.WriteLine(color);
        }
        Console.WriteLine();

    }

    public bool WinCheck()
    {
        Turn currentTurn = TurnsHistory;
        
        int matches = 0; //number of black colors
        int size = currentTurn.TurnMatches.Count; // numbers of matches
        if (size == GameSize.NumberOfCodes) //max black matches required for win, otherwise skip
        {
            for (int i = 0; i < size; i++ ) 
            {
                if (currentTurn.TurnMatches[i] == "black")
                { 
                    matches++;
                }
                        
                if (matches == GameSize.NumberOfCodes)
                {
                    MainClass.MainEnemyDeck = EnemyDeck; //save enemy deck for further display after the game
                    return true; //1 means win
                }
            }
        }
        return false;
    }


}

class MainClass
{
    public static int WinAmount;
    public static List<string> MainEnemyDeck;

    public static bool GameOver(bool result)
     {
         if (result)
         {
            Console.WriteLine("Congratulations! You win!\n");
            WinAmount++;
         }
         else
         {
             Console.WriteLine("Unfortunately you have lost.\n");
         }
         
         Console.WriteLine("Enemy Code: "); //write enemy deck
         foreach (string i in MainEnemyDeck)
         {
            Console.Write(i + " "); 
         }
         Console.WriteLine("\n");
         
         
         Console.WriteLine("Amount of victories = " + WinAmount);
         Console.WriteLine("Do you want to play again? Y/N");
         string answer = Console.ReadLine();
         if (answer is "Y" or "y")
         {
             return true;
         }
         
         return false;
         
         
     }
    
    
    public static void Main()
    {
        bool gameResult;
        
        Console.WriteLine("Welcome to the MasterMind game!\n" +
                          "You have red, green, blue, grey, brown and orange colors.\n" +
                          $"You have {GameSize.NumberOfTurns} attempts.\n" +
                          $"Your enemy has {GameSize.NumberOfCodes} colors you should guess.\n" +
                          "Black = your guess matches both position and color.\n" +
                          "White = you and enemy have common color.\n" +
                          $"You goal is to get {GameSize.NumberOfCodes} black codes");
        
        
        do
        {
            Console.WriteLine("\n");
            gameResult = false;
            Game game = new Game();


            Game.RandomColorFill(game.EnemyDeck, Colors.ColorList, GameSize.NumberOfCodes); //enemy turn
            
            // Console.WriteLine("EnemyDeck: "); // debug enemy deck
            // foreach (string i in game.EnemyDeck)
            // {
            //     Console.Write(i + " "); 
            // }
            // Console.WriteLine("\n");

            for (int i = 0; i < GameSize.NumberOfTurns; i++)
            {
                game.MakeTurn();
                game.MatchCheck();
                gameResult = game.WinCheck();
                if (gameResult) //end game if win
                {
                    break;
                }
                
                MainEnemyDeck = game.EnemyDeck; //save enemy deck in case of defeat for further display

            }
        } while (GameOver(gameResult));
        
    }
}