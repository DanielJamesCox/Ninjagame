using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushToJumpBehavior : MonoBehaviour {

    public EnemyStateMachine partner;

    private enum PartnerType {
        SWORDMAN,
        ASPECT,
        EMPEROR,
        TRUE_EMPEROR
    }

    [SerializeField]
    PartnerType type = PartnerType.SWORDMAN;

    private void Start()
    {
    }
    public void ToggleActive() {
        GetComponent<PointEffector2D>().enabled = !GetComponent<PointEffector2D>().enabled;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GetComponent<PointEffector2D>().enabled) {
            switch (type)
            {
                case PartnerType.SWORDMAN:
                    partner.GetComponent<SwordmanStateMachine>().Jump();
                    break;
                case PartnerType.ASPECT:
                    partner.GetComponent<EndBossBehavior>().MinorJump();
                    break;
                case PartnerType.EMPEROR:
                    partner.GetComponent<EndBossBehavior>().MinorJump();
                    break;
                case PartnerType.TRUE_EMPEROR:
                    partner.GetComponent<EndBossBehavior>().MajorJump();
                    break;
            }
        }
    }
}
