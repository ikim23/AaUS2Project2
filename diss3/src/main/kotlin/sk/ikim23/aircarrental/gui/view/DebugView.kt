package sk.ikim23.aircarrental.gui.view

import javafx.geometry.Orientation
import javafx.scene.control.TableView
import javafx.scene.layout.Priority
import javafx.scene.paint.Color
import sk.ikim23.aircarrental.gui.controller.MainController
import sk.ikim23.aircarrental.gui.model.BusModel
import sk.ikim23.aircarrental.gui.model.ServiceDeskModel
import tornadofx.*

class DebugView(controller: MainController): View() {
    override val root = splitpane(Orientation.VERTICAL)

    init {
        root.hgrow = Priority.ALWAYS
        root.vgrow = Priority.ALWAYS
        tableview(controller.buses) {
            column("#", BusModel::id)
            column("Position", BusModel::position)
            column("Passengers", BusModel::actOccupation)
            column("# rounds", BusModel::rounds)
            column("Average usage", BusModel::avgOccupation)
            column("Confidence interval lower", BusModel::lowOccupation)
            column("Confidence interval upper", BusModel::uppOccupation)
            columnResizePolicy = TableView.CONSTRAINED_RESIZE_POLICY
        }
        tableview(controller.serviceDesks) {
            column("#", ServiceDeskModel::id)
            column("Passenger", ServiceDeskModel::passenger).cellFormat {
                text = it.toString()
                style {
                    if (it == "free") {
                        backgroundColor += c("#43A047")
                        textFill = Color.WHITE
                    }
                }
            }
            column("Average usage", ServiceDeskModel::avgUsage)
            column("Confidence interval lower", ServiceDeskModel::lowBound)
            column("Confidence interval upper", ServiceDeskModel::uppBound)
            columnResizePolicy = TableView.CONSTRAINED_RESIZE_POLICY
        }
    }
}