using System;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    /// <summary>
    /// Данный метод вызывается автоматически при клике на кнопки с изображениями тайлов.
    /// В качестве параметра передается префаб тайла, изображенный на кнопке.
    /// Вы можете использовать префаб tilePrefab внутри данного метода.
    /// </summary>
    
    [SerializeField] 
    private Grid _grid;
    [SerializeField] 
    private int _fieldSizeX = 10;
    [SerializeField] 
    private int _fieldSizeZ = 10;
    [SerializeField]
    private Color _permitOverlayColor;
    [SerializeField]
    private Color _denyOverlayColor;
    [SerializeField]
    private Color _defaultColor;

    private GameObject _tileInstance;
    private bool[,] _occupiedСells;
    private Renderer[] _tileElements;
    private Camera _mainCamera;
    private Ray _ray;
    private Vector3 _cursorPosition;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _occupiedСells = new bool[_fieldSizeX, _fieldSizeZ];
    }

    public void StartPlacingTile(GameObject tilePrefab)
    {
        _tileInstance = Instantiate(tilePrefab);
        _tileElements = _tileInstance.GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        HandleTilePlacement();
    }

    private void HandleTilePlacement()
    {
        if (_tileInstance == null) return;
        _cursorPosition = GetCursorPositionInGrid();
        _tileInstance.transform.position = _cursorPosition;
            
        SetColor(IsTilePlacementValid() ? _permitOverlayColor : _denyOverlayColor);
            
        if (Input.GetMouseButtonDown(0))
        {
            InstantiateTile();
        }
    }

    private void InstantiateTile()
    {
        if (!IsTilePlacementValid()) return;
        var cellPosition = _grid.WorldToCell(_cursorPosition);
        _occupiedСells[cellPosition.x, cellPosition.z] = true;
        var tile = Instantiate(_tileInstance);
        tile.transform.position = _cursorPosition ;
        SetColor(_defaultColor);
        _tileInstance = null;
    }
    
    private Vector3 GetCursorPositionInGrid()
    {
        _ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(_ray, out var hitInfo)) return new Vector3();
        var worldPosition = hitInfo.point;
        var cellPosition = _grid.WorldToCell(worldPosition);
        var cellCenterPosition = _grid.GetCellCenterWorld(cellPosition);
        return cellCenterPosition;
    }

    private bool IsTilePlacementValid()
    {
        var cellInFiledArea = IsPositionInField();
        var cellPosition = _grid.WorldToCell(_cursorPosition);
        bool isCellFree;
        
        try
        {
            isCellFree = _occupiedСells[cellPosition.x, cellPosition.z];
        }
        catch (Exception)
        {
            return false;
        }

        return cellInFiledArea && !isCellFree;
    }
    
    
    private bool IsPositionInField()
    {
        return _cursorPosition.x >= -_fieldSizeX / 2 && _cursorPosition.x <= _fieldSizeX / 2 &&
               _cursorPosition.z >= -_fieldSizeZ / 2 && _cursorPosition.z <= _fieldSizeZ / 2;
    }
    
    private void SetColor(Color color)
    {
        foreach (var tileElement in _tileElements)
        {
            tileElement.material.color = color;
        }
    }
}