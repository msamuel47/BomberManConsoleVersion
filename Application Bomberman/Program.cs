using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Application_Bomberman
{
    class Program
    {
        #region constantes (paramètres du jeu)

        private static Random rnd = new Random(); // Create a new random

        private const int nb_lignes = 15;
        private const int nb_colones = 70;
        private const int difficulteIA = 10; // plus le chiffre est élévé plus le jeu est facile
        private static int compteurDeTour = 0; //Pour compter chaque tour
        private static int positionJoueurX = 0;
        private static int positionJoueurY = 0;
        private const int nb_mursAleatoireAGenerer = 0;
        private const int nb_enemies = 10;

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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compteurTour"></param>
        static void DeplacerIA()
        {
            int positionIAX = 0;
            int positionIAY = 0;
            if (compteurDeTour >= difficulteIA)
            {
                compteurDeTour = 0;
           
                
                for (int i = 0; i < tableauDeJeu.GetLength(0); i++) //Scan X
                {
                    
                    for (int j = 0; j < tableauDeJeu.GetLength(1); j++) //Scan Y
                    {
                        if (tableauDeJeu[i, j] == GameObject.OBJECT_AI_ENEMY)
                        {
                            tableauDeJeu[i,j] = GameObject.OBJECT_NOTHING;
                            positionIAY = j;
                            positionIAX = i;
                            
                            if (j < positionJoueurY)
                            {
                                positionIAY = j + 1;
                            }
                            if (j > positionJoueurY)
                            {
                                positionIAY = j - 1;
                            }
                            if (j == positionJoueurY)
                            {
                                if (positionIAX > positionJoueurX)
                                {
                                    positionIAX = i - 1;
                                }
                                if (positionIAX < positionJoueurX)
                                {
                                    positionIAX = i + 1;
                                }
                            }
                            tableauDeJeu[positionIAX, positionIAY] = GameObject.OBJECT_AI_ENEMY;
                        }
                        
                    }
                    
                }
            }
        }
    }
}