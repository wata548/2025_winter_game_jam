using System;
using System.Collections.Generic;
using System.Linq;
using Extension.Test;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Stage {
    public class StageManager: MonoBehaviour {
        
        //==================================================||Constance 
        private const int MAX_STAGE_COUNT = 5;
           
        //==================================================||Fields 
        [SerializeField] private Stage _bossStage;
        [SerializeField] private Stage _tutorialStage;
        private List<Stage> _stages = null;
        private Stage _curStage = null;
        private int _stageCnt = 0;
        
       //==================================================||Properties 
       public static StageManager Instance { get; private set; } = null;

        [TestMethod]
        public void NextStage() {
                
            _stageCnt++;
            if(_stageCnt == MAX_STAGE_COUNT + 1)
                SetStage(_bossStage);
            else
                SetStage(_stages[Random.Range(0, _stages.Count)]);
        }

        private void SetStage(Stage pStage) {
            Fade.Instance.Change(1, 0.5f, () => {

                Fade.Instance.Change(0, 0.4f);
                if (_curStage != null) {
                    Destroy(_curStage.gameObject);
                    _curStage.RemoveAllEnemies();
                }
                _curStage = Instantiate(pStage);
            });
        }

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            _stages ??= Resources.LoadAll<Stage>("Stage").ToList();
            SetStage(_tutorialStage);
        }
    }
}