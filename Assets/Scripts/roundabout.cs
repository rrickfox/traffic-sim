using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using static Pathfinding.Pathfinding;

//erzeugt eine spezielle T-kreuzug ("protoRoundabout"), aus der Kreisverkehre gebaut werden können. Da ein Kreisverkehr nur in eine Richtung befahrbar ist, bedeutet das für die T-kreuzung, dass eine Straße beliebeige Gestalt haben darf, die anderen beiden jedoch Einbahnstraßen sein müssen (eine von der Kreuzung weg, die andere zur Kreuzung hin)

public class protoRoundabout : MonoBehaviour
{
    public GameObject roadPrefab;
    public GameObject carPrefab;

}