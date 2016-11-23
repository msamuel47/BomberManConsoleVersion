using System;
using System.Threading;


namespace Application_Bomberman
{
    class Program
    {
        #region constantes (paramètres du jeu)

        private static Random rnd = new Random(); // Create a new random

        private const int nb_lignes = 15;
        private const int nb_colones = 50;
        private const int difficulteIA = 2; // plus le chiffre est élévé plus le jeu est facile
        private static int compteurDeTour = 0; //Pour compter chaque tour
        private static int positionJoueurX = 0;
        private static int positionJoueurY = 0;
        private const int nb_mursAleatoireAGenerer = 150;
        private const int nb_enemies = 2;

        private static GameObject[,] tableauDeJeu = new GameObject[nb_colones, nb_lignes];
        //Declaration du tableau de jeu

        #endregion

        static void Main(string[] args)
        {
            positionJoueurX = tableauDeJeu.GetLength(0)/2; // Place le joueur au centre de la cartea
            positionJoueurY = tableauDeJeu.GetLength(1)/2;
            Console.WindowWidth = nb_colones + 1;
            Console.WindowHeight = nb_lignes + 1;


            bool joueurEnVie = true;
            GenererTableauDeJeu();
            AfficherLeTableauDeJeu();
            Console.SetCursorPosition(1, 1);

            while (true)
                {
                    joueurEnVie = EvaluerEtatJoueur();
                    Deplacement saisie = SaisirCoupJoueur();
                    if (saisie != Deplacement.MOVE_NOMOVE)
                        {
                            DeplacerJoueur(saisie);
                            while (Console.KeyAvailable)
                                {
                                    Console.ReadKey(true);
                                }
                            DeplacerIA();
                        }
                    AfficherLeTableauDeJeu();
                    joueurEnVie = EvaluerEtatJoueur();
                    if (joueurEnVie == false)
                        {
                            SignalerFinDePartie();
                            break;
                        }
                }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Clear();
            Console.WriteLine("Vous êtes mort :( ");
            Console.ReadKey();
        }

        /// <summary>
        /// Cette fonction sert à saisir les déplacments du joueur
        /// </summary>
        /// <returns>L'entré du joueur</returns>
        static Deplacement SaisirCoupJoueur()

        {
            Deplacement retour = Deplacement.MOVE_NOMOVE;
            char entrerJoueur = Console.ReadKey(true).KeyChar;
            if (entrerJoueur == 'w')
                {
                    retour = Deplacement.MOVE_UP;
                }
            if (entrerJoueur == 'a')
                {
                    retour = Deplacement.MOVE_LEFT;
                }
            if (entrerJoueur == 's')
                {
                    retour = Deplacement.MOVE_DOWN;
                }
            if (entrerJoueur == 'd')
                {
                    retour = Deplacement.MOVE_RIGHT;
                }
            if (entrerJoueur == ' ')
                {
                    retour = Deplacement.MOVE_BOMBPLACEMENT;
                }

            return retour;
        }

        /// <summary>
        /// Genère le tableau du jeu en créant les murs et la génération des
        /// </summary>
        static void GenererTableauDeJeu()
        {
            //Empty array for dumping the old game
            for (int i = 0; i < tableauDeJeu.GetLength(0); i++)
                {
                    for (int j = 0; j < tableauDeJeu.GetLength(1); j++)
                        {
                            tableauDeJeu[i, j] = GameObject.OBJECT_NOTHING; //Assign OBJECT_NOTHING
                        }
                }

            //Create outter walls 
            for (int i = 0; i < tableauDeJeu.GetLength(0); i++)
                {
                    tableauDeJeu[i, 0] = GameObject.OBJECT_WALL;
                }
            for (int i = 0; i < tableauDeJeu.GetLength(1); i++)
                {
                    tableauDeJeu[0, i] = GameObject.OBJECT_WALL;
                }
            for (int i = tableauDeJeu.GetLength(0) - 1; i > 0; i--)
                {
                    tableauDeJeu[i, tableauDeJeu.GetLength(1) - 1] = GameObject.OBJECT_WALL;
                }
            for (int i = tableauDeJeu.GetLength(1) - 1; i > 0; i--)
                {
                    tableauDeJeu[tableauDeJeu.GetLength(0) - 1, i] = GameObject.OBJECT_WALL;
                }

            //Place player in center
            tableauDeJeu[positionJoueurX, positionJoueurY] = GameObject.OBJECT_PLAYER;

            //Start generate random walls in array
            for (int i = 0; i < nb_mursAleatoireAGenerer; i++)
                {
                    int valeurX = rnd.Next(0, tableauDeJeu.GetLength(0) - 1);
                    int valeurY = rnd.Next(0, tableauDeJeu.GetLength(1) - 1);
                    while (tableauDeJeu[valeurX, valeurY] == GameObject.OBJECT_WALL ||
                           //If the player or a wall is already there
                           tableauDeJeu[valeurX, valeurY] == GameObject.OBJECT_PLAYER) // Choose antother position
                        {
                            valeurX = rnd.Next(0, tableauDeJeu.GetLength(0) - 1);
                            valeurY = rnd.Next(0, tableauDeJeu.GetLength(1) - 1);
                        }
                    tableauDeJeu[valeurX, valeurY] = GameObject.OBJECT_WALL;
                }

            //Start placing enemies
            for (int i = 0; i < nb_enemies; i++)
                {
                    int valeurX = rnd.Next(0, tableauDeJeu.GetLength(0) - 1);
                    int valeurY = rnd.Next(0, tableauDeJeu.GetLength(1) - 1);
                    while (tableauDeJeu[valeurX, valeurY] == GameObject.OBJECT_WALL ||
                           //If the player or a wall is already there
                           tableauDeJeu[valeurX, valeurY] == GameObject.OBJECT_PLAYER) // Choose antother position
                        {
                            valeurX = rnd.Next(0, tableauDeJeu.GetLength(0) - 1);
                            valeurY = rnd.Next(0, tableauDeJeu.GetLength(1) - 1);
                        }
                    tableauDeJeu[valeurX, valeurY] = GameObject.OBJECT_AI_ENEMY;
                }
        }

        /// <summary>
        /// Drawing of the array in the console
        /// </summary>
        static void AfficherLeTableauDeJeu()
        {
            for (int i = 0; i < tableauDeJeu.GetLength(0); i++)
                {
                    for (int j = 0; j < tableauDeJeu.GetLength(1); j++)
                        {
                            Console.SetCursorPosition(i, j);
                            if (tableauDeJeu[i, j] == GameObject.OBJECT_WALL)
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                    Console.Write("#");
                                    Console.ResetColor();
                                }
                            if (tableauDeJeu[i, j] == GameObject.OBJECT_NOTHING)
                                {
                                    Console.Write(" ");
                                }
                            if (tableauDeJeu[i, j] == GameObject.OBJECT_AI_ENEMY)
                                {
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    Console.Write("*");
                                    Console.ResetColor();
                                }
                            if (tableauDeJeu[i, j] == GameObject.OBJECT_PLAYER)
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.Write("@");
                                    Console.ResetColor();
                                }
                            if (tableauDeJeu[i, j] == GameObject.OBJECT_BOMB)
                                {
                                    Console.BackgroundColor = ConsoleColor.Red;
                                    Console.Write(" ");
                                    Console.ResetColor();
                                }
                        }
                }
        }

        /// <summary>
        /// Determine si le joueur peu se déplacer et update sa position si possible
        /// </summary>
        /// <param name="entreJoueur"></param>
        static void DeplacerJoueur(Deplacement entreJoueur)
        {
            if (entreJoueur == Deplacement.MOVE_UP)
                {
                    if (tableauDeJeu[positionJoueurX, positionJoueurY - 1] != GameObject.OBJECT_WALL)
                        {
                            tableauDeJeu[positionJoueurX, positionJoueurY] = GameObject.OBJECT_NOTHING;
                            positionJoueurY--;
                            tableauDeJeu[positionJoueurX, positionJoueurY] = GameObject.OBJECT_PLAYER;
                        }
                }
            if (entreJoueur == Deplacement.MOVE_LEFT)
                {
                    if (tableauDeJeu[positionJoueurX - 1, positionJoueurY] != GameObject.OBJECT_WALL)
                        {
                            tableauDeJeu[positionJoueurX, positionJoueurY] = GameObject.OBJECT_NOTHING;
                            positionJoueurX--;
                            tableauDeJeu[positionJoueurX, positionJoueurY] = GameObject.OBJECT_PLAYER;
                        }
                }
            if (entreJoueur == Deplacement.MOVE_RIGHT)
                {
                    if (tableauDeJeu[positionJoueurX + 1, positionJoueurY] != GameObject.OBJECT_WALL)
                        {
                            tableauDeJeu[positionJoueurX, positionJoueurY] = GameObject.OBJECT_NOTHING;
                            positionJoueurX++;
                            tableauDeJeu[positionJoueurX, positionJoueurY] = GameObject.OBJECT_PLAYER;
                        }
                }
            if (entreJoueur == Deplacement.MOVE_DOWN)
                {
                    if (tableauDeJeu[positionJoueurX, positionJoueurY + 1] != GameObject.OBJECT_WALL)
                        {
                            tableauDeJeu[positionJoueurX, positionJoueurY] = GameObject.OBJECT_NOTHING;
                            positionJoueurY++;
                            tableauDeJeu[positionJoueurX, positionJoueurY] = GameObject.OBJECT_PLAYER;
                        }
                }
            if (entreJoueur == Deplacement.MOVE_BOMBPLACEMENT)
                {
                    ExploserBombe();
                }
            compteurDeTour++;
        }

        /// <summary>
        /// Fait exploser la bombe
        /// </summary>
        static void ExploserBombe()
        {
            tableauDeJeu[positionJoueurX + 1, positionJoueurY - 1] = GameObject.OBJECT_NOTHING;
            tableauDeJeu[positionJoueurX + 1, positionJoueurY] = GameObject.OBJECT_NOTHING;
            tableauDeJeu[positionJoueurX + 1, positionJoueurY + 1] = GameObject.OBJECT_NOTHING;
            tableauDeJeu[positionJoueurX, positionJoueurY + 1] = GameObject.OBJECT_NOTHING;
            tableauDeJeu[positionJoueurX, positionJoueurY - 1] = GameObject.OBJECT_NOTHING;
            tableauDeJeu[positionJoueurX - 1, positionJoueurY - 1] = GameObject.OBJECT_NOTHING;
            tableauDeJeu[positionJoueurX - 1, positionJoueurY] = GameObject.OBJECT_NOTHING;
            tableauDeJeu[positionJoueurX - 1, positionJoueurY + 1] = GameObject.OBJECT_NOTHING;
        }

        static bool EvaluerEtatJoueur()
        {
            bool condition = !(tableauDeJeu[positionJoueurX + 1, positionJoueurY - 1] == GameObject.OBJECT_AI_ENEMY ||
                               tableauDeJeu[positionJoueurX + 1, positionJoueurY] == GameObject.OBJECT_AI_ENEMY ||
                               tableauDeJeu[positionJoueurX + 1, positionJoueurY + 1] == GameObject.OBJECT_AI_ENEMY ||
                               tableauDeJeu[positionJoueurX, positionJoueurY + 1] == GameObject.OBJECT_AI_ENEMY ||
                               tableauDeJeu[positionJoueurX, positionJoueurY - 1] == GameObject.OBJECT_AI_ENEMY ||
                               tableauDeJeu[positionJoueurX - 1, positionJoueurY - 1] == GameObject.OBJECT_AI_ENEMY ||
                               tableauDeJeu[positionJoueurX - 1, positionJoueurY] == GameObject.OBJECT_AI_ENEMY ||
                               tableauDeJeu[positionJoueurX - 1, positionJoueurY + 1] == GameObject.OBJECT_AI_ENEMY);
            return condition;
        }

        static void SignalerFinDePartie()
        {
            Console.Clear();
            Console.WriteLine("Vous êtes mort ...");
            Thread.Sleep(500);
        }

        /// <summary>
        /// Use to move every AI on the game by using the position of the player this method is using a shared 2D Array
        /// this method is bugged when the player is UNDER the enemies ... They all disapeard for no reason and
        /// I am not able to find what is wrond with it
        /// </summary>
        static void DeplacerIA()
        {
            if (compteurDeTour >= difficulteIA)
                {
                    compteurDeTour = 0;
                     GameObject[,] tableauDeMemoire = CopierTableau(tableauDeJeu);

                    for (int i = 0; i < tableauDeJeu.GetLength(0); i++) //Scan X
                        {

                            for (int j = 0; j < tableauDeJeu.GetLength(1); j++) //Scan Y
                                {
                                    int positionIAY = j; //Assign variable to memorise the AI position for modification
                                    int positionIAX = i;
                                    if (tableauDeJeu[i, j] == GameObject.OBJECT_AI_ENEMY)
                                        // If enemy found in the array scan
                                        {
                                            tableauDeMemoire[i, j] = GameObject.OBJECT_NOTHING;
                                                //Erease is first location for refresh
                                            //Assign variable to memorise the AI position for modification
                                            if (positionIAY > positionJoueurY) //If the AI is below the player
                                                {
                                                    
                                                            positionIAY--;
                                                                             //Decrease is position from 1
                                                }
                                            else if (positionIAY < positionJoueurY)
                                                {
                                                    positionIAY++; //Increase is position from 1
                                                }

                            if (positionIAY == positionJoueurY)  //If the AI is at the same elevation of the player
                            { //Scan for his horizontal situation
                                if (positionIAX > positionJoueurX)  //If the AI is at the right of the player
                                {
                                    positionIAX = i - 1;  //Move it left 
                                }
                                if (positionIAX < positionJoueurX) // If the AI is at the left of the player
                                {
                                    positionIAX = i + 1; //Move it right
                                }
                            }
                            tableauDeMemoire[positionIAX, positionIAY] = GameObject.OBJECT_AI_ENEMY;
                                                //Assign the new position
                                        } // To the enemy
                                }

                        }
                    tableauDeJeu = CopierTableau(tableauDeMemoire);
                }
        }

        static GameObject[,] CopierTableau(GameObject[,] tableauACopie)
        {
            GameObject[,] TableauCopie = new GameObject[tableauACopie.GetLength(0),tableauACopie.GetLength(1)];
            for (int i = 0; i < TableauCopie.GetLength(0); i++)
                {
                    for (int j = 0; j < TableauCopie.GetLength(1); j++)
                        {
                            TableauCopie[i, j] = tableauACopie[i, j];
                        }
                }
            return TableauCopie;
        }
       
    }
}