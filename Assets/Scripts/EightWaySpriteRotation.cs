using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EightWaySpriteRotation : MonoBehaviour
{
    public SpriteRenderer characterSprite;
    public bool doFullRotation = true;
    public bool flipSprite = true;
    public bool rotateSpriteRenderer = false;

    public Sprite dSprite;
    public Sprite dRSprite;
    public Sprite rSprite;
    public Sprite uRSprite;
    public Sprite uSprite;

    public GameObject FacingDirectionSource;
    private IFacingDirection _facingDirectionInterface;

    private void Awake()
    {
        _facingDirectionInterface = FacingDirectionSource.GetComponent<IFacingDirection>();
    }

    public void Update()
    {
        if (FacingDirectionSource != null)
        {
            Vector2 facingDirection = _facingDirectionInterface.GetFacingDirection();

            if (facingDirection != Vector2.zero)
            {
                float signedAngle = Vector2.SignedAngle(Vector2.up, facingDirection);

                if (rotateSpriteRenderer)
                {
                    characterSprite.transform.localEulerAngles = new Vector3(0, 0, signedAngle);
                }

                if (flipSprite)
                {
                    characterSprite.transform.localScale = new Vector3((signedAngle > 0 ? -1 : 1) * Mathf.Abs(characterSprite.transform.localScale.x), characterSprite.transform.localScale.y, characterSprite.transform.localScale.z);
                }

                if (doFullRotation)
                {
                    float unsignedAngle = Mathf.Abs(signedAngle);

                    if (unsignedAngle < 22.5f)
                    {
                        characterSprite.sprite = uSprite;
                    }
                    else if (unsignedAngle < 67.5f)
                    {
                        characterSprite.sprite = uRSprite;
                    }
                    else if (unsignedAngle < 112.5f)
                    {
                        characterSprite.sprite = rSprite;
                    }
                    else if (unsignedAngle < 157.5f)
                    {
                        characterSprite.sprite = dRSprite;
                    }
                    else
                    {
                        characterSprite.sprite = dSprite;
                    }
                }
            }
        }
    }
}