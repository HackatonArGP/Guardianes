using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Earthwatchers.Models
{
    public enum EwFeature
    {
        ForestLaw,
        JaguarGame,
        Collections,
        Polls,
        Image2008Warning
    }

    public class Features : Dictionary<EwFeature, int>
    {
        private List<Score> _userScores = null;
        private List<EwFeature> _prevFeatures = null;
        private int _earthwatcherId = 0;

        public Features(List<Score> scores, int earthwatcherId)
        {
            this._userScores = scores;
            this._prevFeatures = new List<EwFeature>();
            this._earthwatcherId = earthwatcherId;
            LoadAllFeatures();
            LoadPreviousFeatures();
        }
        

        public List<EwFeature> AllFeatures
        {
            get
            {
                return this.Keys.ToList();
            }
        }

        public List<EwFeature> UnlockedFeatures
        {
            get
            {
                return AllFeatures.Where(f => IsUnlocked(f)).ToList();
            }
        }

        public List<EwFeature> NewFeatures 
        {
            //solo features desbloquedos durante esta sesion.
            get
            {
                List<EwFeature> newFeatures = new List<EwFeature>();
                if (HasNewUnlocks())
                {
                    newFeatures = UnlockedFeatures.Except(_prevFeatures).ToList();
                }
                return newFeatures;
            }
        }
        


        public bool IsLogged(EwFeature feature)
        {
            var logged = false;
            var relatedAction = GetRelatedAction(feature);
            if (relatedAction.HasValue)
            {
                if (_userScores.Any(s => s.Action == relatedAction.Value.ToString()))
                {
                    logged = true;
                }
            }

            return logged;
        }

        public bool IsUnlocked(EwFeature feature)
        {
            var unlocked = false;
            if (NeededPoints(feature) == 0)
            {
                unlocked = true;
            }

            return unlocked;
        }

        public bool IsNew(EwFeature feature)
        {
            return NewFeatures.Any(f => f == feature);
        }
        
        public int NeededPoints(EwFeature feature)
        {
            int remaining = 0;
            int featurePoints = PointsToUnlock(feature);
            if (featurePoints > 0)
            {
                var userPoints = _userScores.Sum(s => s.Points);
                if (featurePoints > userPoints)
                {
                    remaining = featurePoints - userPoints;
                }
            }

            return remaining;
        }

        public List<Score> GetNewUnlocksToLog()
        {
            var scores = new List<Score>();

            var unlocksToLog = UnlockedFeatures.Where(f => !IsLogged(f));
            foreach (var feature in unlocksToLog)
            {
                var action = GetRelatedAction(feature);
                if (action.HasValue)
                {
                    scores.Add(new Score() 
                    { 
                        Action = action.Value.ToString(), 
                        Points = ActionPoints.Points(action.Value),
                        EarthwatcherId = _earthwatcherId
                    });
                }
            }

            return scores;
        }


        private void LoadAllFeatures()
        {
            this.Add(EwFeature.ForestLaw, 2000000000); 
            this.Add(EwFeature.JaguarGame, 5000);
            this.Add(EwFeature.Collections, 2000000000); //TODO: puntaje original 7000
            this.Add(EwFeature.Polls, 10000);
            this.Add(EwFeature.Image2008Warning, 3000);
        }

        private void LoadPreviousFeatures()
        {
            var login = _userScores.Where(s => s.Action == ActionPoints.Action.Login.ToString()).OrderByDescending(s => s.Published).FirstOrDefault();
            if (login != null)
            {
                var lastLoginScores = _userScores.Where(s => s.Published < login.Published);
                foreach (var feature in AllFeatures)
                {
                    var relatedAction = GetRelatedAction(feature);
                    if (relatedAction.HasValue)
                    {
                        if (lastLoginScores.Any(s => s.Action == relatedAction.Value.ToString()))
                        {
                            _prevFeatures.Add(feature);
                        }
                    }
                }
            }
        }

        private int PointsToUnlock(EwFeature feature)
        {
            return this.ContainsKey(feature) ? this[feature] : 0;
        }

        private ActionPoints.Action? GetRelatedAction(EwFeature feature)
        {
            ActionPoints.Action? action = null;
            switch (feature)
            {
                case EwFeature.ForestLaw: action = ActionPoints.Action.FeatureUnlocked_ForestLaw; break;
                case EwFeature.JaguarGame: action = ActionPoints.Action.FeatureUnlocked_JaguarGame; break;
                case EwFeature.Collections: action = ActionPoints.Action.FeatureUnlocked_Collections; break;
                case EwFeature.Polls: action = ActionPoints.Action.FeatureUnlocked_Polls; break;
                case EwFeature.Image2008Warning: action = ActionPoints.Action.FeatureUnlocked_Image2008Warning; break;
                default: action = null; break;
            }
            return action;
        }

        private bool HasNewUnlocks()
        {
            return UnlockedFeatures.Count > _prevFeatures.Count;
        }
                
    }
}
