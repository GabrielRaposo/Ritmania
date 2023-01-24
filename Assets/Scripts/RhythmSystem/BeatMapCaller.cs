using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmSystem 
{
    public class BeatMapCaller : MonoBehaviour
    {
        public BeatMapData beatMapData;  

        Conductor conductor;

        void Start()
        {
            // Verifica a validade dos dados de Beatmap que v�o ser utilizados
            if (!beatMapData || !beatMapData.Music || beatMapData.BPM <= 0)
            {
                Debug.LogError("Invalid beatmap data.");
                enabled = false;
                return;
            }

            // Verifica se existe um condutor na cena
            conductor = Conductor.Instance;
            if (!conductor) 
            {
                Debug.LogError("Conductor couldn't be found.");
                enabled = false;
                return;
            }

            conductor.SetupConductor(beatMapData);
        }

        void Update()
        {
            // Aguarda pelo input do jogador para come�ar a rodar
            if (!conductor.isRunning)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                     conductor.StartConduction();
            }    
        }
    }
}
