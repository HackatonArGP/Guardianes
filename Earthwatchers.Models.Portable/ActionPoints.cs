using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Earthwatchers.Models
{
    public static class ActionPoints
    {
        public enum Action
        {
            Login,
            LandVerified,
            ConfirmationAdded,
            LandStatusChanged,
            MiniJuegoI,
            LandReassigned,
            FoundTheJaguar,
            CollectionComplete,
            FivePollAnswersBonus,
            TutorialCompleted,
            LandVerifiedInformed,
            DemandAuthorities,
            Shared,
            ScoringHelp,
            FeatureUnlocked_ForestLaw,
            FeatureUnlocked_JaguarGame,
            FeatureUnlocked_Collections,
            FeatureUnlocked_Polls,
            PreTutorialStep1,
            PreTutorialStep2,
            PreTutorialStep3,
            PreTutorialStep4,
            TutorialStep0,
            TutorialStep1,
            TutorialStep2,
            TutorialStep3,
            TutorialStep4,
            TutorialStep5, 
            TutorialStep6,
            FeatureUnlocked_Image2008Warning,
            DailyMessage,
            DailySummary,
            ContestWon,
            ContestWinnerAnnounced,
            LandReseted,
            Log
        }

        public static int Points(Action action)
        {
            int points = 0;
            switch (action)
            {
                case Action.Login: points = 100; break;
                case Action.LandVerified: points = 1000; break; //Cuando la parcela que verificaste llega a las 30 verificaciones
                case Action.ConfirmationAdded: points = 250; break; //Cuando se revisa la parcela de otro usuario
                case Action.LandStatusChanged: points = 750; break; // Cuando revisas tu propia parcela
                case Action.MiniJuegoI: points = 500; break; 
                case Action.LandReassigned: points = 0; break; //Cuando se hace el changeLand
                case Action.FoundTheJaguar: points = 5000; break;
                case Action.CollectionComplete: points = 5000; break;
                case Action.FivePollAnswersBonus: points = 1000; break;
                case Action.TutorialCompleted: points = 2000; break;
                case Action.LandVerifiedInformed: points = 0; break; //Cuando te llega el cartelito porque te informaron que tu parcela fue verificada por greenpeace
                case Action.DemandAuthorities: points = 1000; break; //Cuando denuncias
                case Action.Shared: points = 500; break;  //Cuando compartis
                case Action.ContestWon: points = 10000; break; //Ganador del concurso
                case Action.ScoringHelp: points = 0; break; //Cuando haces click en el menu de ayuda
                case Action.FeatureUnlocked_ForestLaw: points = 0; break;
                case Action.FeatureUnlocked_JaguarGame: points = 0; break;
                case Action.FeatureUnlocked_Collections: points = 0; break;
                case Action.FeatureUnlocked_Polls: points = 0; break;
                case Action.PreTutorialStep1: points = 0; break;
                case Action.PreTutorialStep2: points = 0; break;
                case Action.PreTutorialStep3: points = 0; break;
                case Action.PreTutorialStep4: points = 0; break;
                case Action.TutorialStep0: points = 0; break;
                case Action.TutorialStep1: points = 0; break;
                case Action.TutorialStep2: points = 0; break;
                case Action.TutorialStep3: points = 0; break;
                case Action.TutorialStep4: points = 0; break;
                case Action.TutorialStep5: points = 0; break;
                case Action.TutorialStep6: points = 0; break;
                case Action.FeatureUnlocked_Image2008Warning: points = 0; break;
                case Action.DailyMessage: points = 0; break; //Cuando te llega el DailyMessage
                case Action.DailySummary: points = 0; break; //Cuando recibis el resumen diario
                case Action.ContestWinnerAnnounced: points = 0; break;
                case Action.LandReseted: points = 0; break;  // ????
                case Action.Log: points = 0; break;

            }

            return points;
        }
    }
}
