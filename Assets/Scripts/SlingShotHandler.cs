using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class SlingShotHandler : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField] private LineRenderer _leftLineRenderer;
    [SerializeField] private LineRenderer _rightLineRenderer;
    [Header("Transform References")]
    [SerializeField] private Transform _leftStartPosition;
    [SerializeField] private Transform _rightStartPosition;
    [SerializeField] private Transform _centerPosition;
    [SerializeField] private Transform _IdlePosition;
    [SerializeField] private Transform _elasticTransForm;

    [Header("SlingShot Stats")]
    [SerializeField] private float _maxDistance = 0.275f;
    [SerializeField] private float _shotForce = 10f;
    [SerializeField] private float _timeBetweenBirdRespawns = 2f;
    [SerializeField] private float _elasticDivider = 1.2f;
    [SerializeField] private AnimationCurve _elasticCurve;

    [Header("Scripts")]
    [SerializeField] private SlingShotArea _slingShotArea;

    [Header("Bird")]
    [SerializeField] private AngryBird _angryBirdPrefab;
    [SerializeField] private float _angryBirdPositionOffset = 2f;

    private Vector2 _slingShotLinesPosition;

    private Vector2 _direction;
    private Vector2 _dircetionNormalized;

    private bool _clickedWithinArea;
    private AngryBird _spawnedAngryBird;
    private bool _birdOnSlingShot;




    private void Awake()
    {
        _leftLineRenderer.enabled = false;
        _rightLineRenderer.enabled = false;

        SpawnAngryBird();
    }

    // Update is called once per frame
    private void Update()
    {
        if (InputManager.WasLeftMouseButtonPressed && _slingShotArea.IsWithinSlingShotArea())
        {
            _clickedWithinArea = true;
        }
        if (InputManager.IsLeftMousePressed && _clickedWithinArea && _birdOnSlingShot)
        {
            DrawSlingShot();
            PositionAndRotateAngryBird();
        }
        if (InputManager.WasLeftMouseButtonReleased && _birdOnSlingShot)
        {
            if (GameManager.instance.HasEnoughShots())
            {
                _clickedWithinArea = false;
                _spawnedAngryBird.LaunchBird(_direction, _shotForce);
                GameManager.instance.UseShot();
                _birdOnSlingShot = false;
                AnimateSlingShot();
                if (GameManager.instance.HasEnoughShots())
                {
                    StartCoroutine(SpawnAngryBirdAfterTime());
                }
            }
        }
        //Debug.Log(Mouse.current.position.ReadValue());
    }

    #region SlingShot methods
    private void DrawSlingShot()
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);
        _slingShotLinesPosition = _centerPosition.position + Vector3.ClampMagnitude(touchPosition - _centerPosition.position, _maxDistance);
        SetLines(_slingShotLinesPosition);
        _direction = (Vector2)_centerPosition.position - _slingShotLinesPosition;
        _dircetionNormalized = _direction.normalized;
    }

    private void SetLines(Vector2 position)
    {
        if (!_leftLineRenderer.enabled && !_rightLineRenderer.enabled)
        {
            _leftLineRenderer.enabled = true;
            _rightLineRenderer.enabled = true;
        }
        _leftLineRenderer.SetPosition(0, position);
        _leftLineRenderer.SetPosition(1, _leftStartPosition.position);
        _rightLineRenderer.SetPosition(0, position);
        _rightLineRenderer.SetPosition(1, _rightStartPosition.position);

    }

    #endregion


    #region Angry Bird Methods

    private void SpawnAngryBird()
    {
        SetLines(_IdlePosition.position);
        Vector2 dir = (_centerPosition.position - _IdlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)_IdlePosition.position + dir * _angryBirdPositionOffset;
        _spawnedAngryBird = Instantiate(_angryBirdPrefab, spawnPosition, Quaternion.identity);
        _spawnedAngryBird.transform.right = _dircetionNormalized;
        _birdOnSlingShot = true;
    }

    private void PositionAndRotateAngryBird()
    {
        _spawnedAngryBird.transform.position = _slingShotLinesPosition + _dircetionNormalized * _angryBirdPositionOffset;
        _spawnedAngryBird.transform.right = _dircetionNormalized;
    }

    private IEnumerator SpawnAngryBirdAfterTime()
    {
        yield return new WaitForSeconds(_timeBetweenBirdRespawns);
        SpawnAngryBird();
    }

    #endregion

    #region SlingShot Animation

    private void AnimateSlingShot()
    {
        _elasticTransForm.position = _leftLineRenderer.GetPosition(0);
        float dist = Vector2.Distance(_elasticTransForm.position, _centerPosition.position);
        float time = dist / _elasticDivider;
        _elasticTransForm.DOMove(_centerPosition.position, time).SetEase(_elasticCurve);
        StartCoroutine(AnimateSlingShotLines(_elasticTransForm, time));
    }

    private IEnumerator AnimateSlingShotLines(Transform trans, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            SetLines(trans.position);
            yield return null;
        }
    }

    #endregion

}
