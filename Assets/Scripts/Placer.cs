using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placer : MonoBehaviour
{
    public List<Behaviour> DisabledComponentsUntilPlaced { get; set; }

    public Bounds Bounds { get; set; }
    public Bounds BoundsCenterWorldSpace { get; set; }

    private Material _errorMaterial;
    private Material _normalMaterial;
    private Renderer _renderer;

    private bool Colliding;


    private AudioClip _errorSound;
    private AudioSource _audioSource;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Placeable")
        {
            Colliding = true;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Placeable")
        {
            Colliding = false;
        }
    }

    void Start()
    {
        _audioSource = GameObject.FindObjectOfType<AudioSource>();
        _errorSound = (AudioClip)Resources.Load("Sounds/Error");

        _normalMaterial = (Material)Resources.Load("Materials/Normal");
        _errorMaterial = (Material)Resources.Load("Materials/Error");
        _renderer = transform.GetComponent<Renderer>();
        _renderer.material = _normalMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if (Colliding)
        {
            _renderer.material = _errorMaterial;
        }
        else
        {
            _renderer.material = _normalMaterial;
        }

        var mousePosition = Input.mousePosition;
        var mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePosition);
        // Only consider x and y for bounds checks
        mousePositionInWorld = new Vector3(mousePositionInWorld.x, mousePositionInWorld.y, Bounds.center.z);

        var mouseOutOfBounds = !Bounds.Contains(mousePositionInWorld);
        var positionOfObjectInWorld = mousePositionInWorld;


        if (mouseOutOfBounds)
        {
            positionOfObjectInWorld = Bounds.ClosestPoint(positionOfObjectInWorld);
        }

        transform.position = new Vector3(positionOfObjectInWorld.x, positionOfObjectInWorld.y, positionOfObjectInWorld.y);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);


        if (Input.GetMouseButton(0))
        {
            if (Colliding || mouseOutOfBounds)
            {
                if (!_audioSource.isPlaying)
                {
                    _audioSource.PlayOneShot(_errorSound);
                }
            }
            else
            {
                foreach (var disabledComponent in DisabledComponentsUntilPlaced)
                {
                    disabledComponent.enabled = true;
                }
                Destroy(this);
            }
        }
        else
        {
            foreach (var disabledComponent in DisabledComponentsUntilPlaced)
            {
                disabledComponent.enabled = false;
            }
        }
    }
}
