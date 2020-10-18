package sk.ikim23.carrental.model

import javafx.beans.property.SimpleIntegerProperty

class InputModel(nReps: Int, nBusFrom: Int, nBusTo: Int, nEmplFrom: Int, nEmplTo: Int) {
    val nReplications = SimpleIntegerProperty(nReps)
    val busFrom = SimpleIntegerProperty(nBusFrom)
    val busTo = SimpleIntegerProperty(nBusTo)
    val employeesFrom = SimpleIntegerProperty(nEmplFrom)
    val employeesTo = SimpleIntegerProperty(nEmplTo)
}