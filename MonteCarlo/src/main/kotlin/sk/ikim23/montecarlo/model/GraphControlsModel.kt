package sk.ikim23.montecarlo.model

import javafx.beans.property.SimpleBooleanProperty
import javafx.beans.property.SimpleIntegerProperty

class GraphControlsModel(nDoors: Int) {
    val doorsProperty = SimpleIntegerProperty(nDoors)
    val keepGuessVisibleProperty = SimpleBooleanProperty(true)
    val changeGuessVisibleProperty = SimpleBooleanProperty(true)
}