using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YuEzTools.Utils;

namespace YuEzTools.UI;

public class NewRequestBanner : MonoBehaviour
{
    public void Update()
    {
        // int totalCount = DestroyableSingleton<FriendsListManager>.Instance.ReceivedLobbyInvites.Count + DestroyableSingleton<FriendsListManager>.Instance.ReceivedRequests.Count;
        activeBanner.SetActive(true);
        base.transform.position = new Vector3(this.parentRenderer.transform.position.x - this.ourSprite.size.x / 2f * this.ourSprite.transform.localScale.x - this.parentRenderer.size.x * 0.4f * this.parentRenderer.transform.localScale.x, base.transform.position.y, base.transform.position.z);
        
        // this.inactiveBanner.SetActive(totalCount > 0);
        this.notifText.ForEach<TMPro.TextMeshPro>(delegate(TextMeshPro t)
        {
            t.text = "test";
            t.transform.position = new Vector3(this.ourSprite.transform.position.x, t.transform.position.y, t.transform.position.z);
        });
    }

    // Token: 0x040019B2 RID: 6578
    // [SerializeField]
    public GameObject activeBanner;

    // Token: 0x040019B3 RID: 6579
    // [SerializeField]
    public GameObject inactiveBanner;

    // Token: 0x040019B4 RID: 6580
    // [SerializeField]
    public SpriteRenderer parentRenderer;

    // Token: 0x040019B5 RID: 6581
    // [SerializeField]
    public SpriteRenderer ourSprite;

    // Token: 0x040019B6 RID: 6582
    // [SerializeField]
    public TextMeshPro[] notifText;
}
