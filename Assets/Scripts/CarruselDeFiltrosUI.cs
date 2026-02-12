using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using TMPro; // Necesario para el texto

public class CarruselDeFiltrosUI : MonoBehaviour
{
    // --- Arrastra tus objetos desde el Editor de Unity ---
    [Header("Referencias de Escena")]
    public ARFaceManager faceManager;
    public TextMeshProUGUI textoDisplay; // El texto "FILTRO 1"

    [Header("Datos de Filtros")]
    // Arrastra tus Sprites (facemap1, facemap2...) aquí
    public List<Sprite> iconosFiltro;

    // Escribe los nombres ("Hombre 1", "Niño"...) aquí
    public List<string> nombresFiltro;
    // ----------------------------------------------------

    private Material materialCara;
    private int indiceActual = 0;

    void Start()
    {
        // Chequeo por si olvidas asignar los iconos
        if (iconosFiltro.Count == 0)
        {
            Debug.LogError("¡No se han asignado iconos de filtro en el script CarruselDeFiltrosUI!");
            return;
        }

        // Se suscribe al evento del Face Manager
        faceManager.trackablesChanged.AddListener(OnCarasCambiadas);

        // Configura el estado inicial con el primer filtro
        ActualizarTexto();
    }

    void OnDestroy()
    {
        // Limpia la subscripción al evento
        faceManager.trackablesChanged.RemoveListener(OnCarasCambiadas);
    }

    // Se llama cuando ARCore detecta o actualiza una cara
    void OnCarasCambiadas(ARTrackablesChangedEventArgs<ARFace> args)
    {
        // Cuando se detecta una cara por primera vez
        foreach (ARFace face in args.added)
        {
            // Guarda el material de la cara
            materialCara = face.GetComponent<MeshRenderer>().material;
            // Aplica la textura que esté seleccionada
            AplicarTexturaActual();
        }

        // Si la cara se actualiza (por si se pierde y recupera)
        foreach (ARFace face in args.updated)
        {
            if (materialCara == null)
            {
                materialCara = face.GetComponent<MeshRenderer>().material;
            }
        }
    }

    /// <summary>
    /// Esta es la función PÚBLICA que llamarán tus botones.
    /// </summary>
    /// <param name="indice">El índice del filtro a seleccionar (0, 1, 2...)</param>
    public void SeleccionarFiltro(int indice)
    {
        // Chequeo de seguridad para evitar errores
        if (indice < 0 || indice >= iconosFiltro.Count)
        {
            Debug.LogWarning("Índice de filtro no válido: " + indice);
            return;
        }

        // 1. Actualizamos el índice
        indiceActual = indice;

        // 2. Aplicamos la textura al material de la cara
        AplicarTexturaActual();

        // 3. Actualizamos el texto en la UI
        ActualizarTexto();
    }

    // Función interna para aplicar la textura
    private void AplicarTexturaActual()
    {
        if (materialCara != null && iconosFiltro.Count > indiceActual)
        {
            // Obtenemos la Textura 2D (que necesita el material) desde el Sprite (que usa la UI)
            materialCara.SetTexture("_BaseMap", iconosFiltro[indiceActual].texture);
        }
    }

    // Función interna para actualizar el texto
    private void ActualizarTexto()
    {
        if (textoDisplay != null && nombresFiltro.Count > indiceActual)
        {
            textoDisplay.text = nombresFiltro[indiceActual];
        }
    }
}