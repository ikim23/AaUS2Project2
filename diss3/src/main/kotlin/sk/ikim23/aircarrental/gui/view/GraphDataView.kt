package sk.ikim23.aircarrental.gui.view

import javafx.scene.control.TableView
import javafx.scene.layout.Priority
import sk.ikim23.aircarrental.gui.controller.MainController
import sk.ikim23.aircarrental.gui.model.GraphDataModel
import tornadofx.*

class GraphDataView(controller: MainController) : View() {
    override val root = vbox()

    init {
        root.hgrow = Priority.ALWAYS
        root.vgrow = Priority.ALWAYS
        tableview(controller.simConfigs) {
            column("# Employees", GraphDataModel::nEmployees)
            column("# Buses", GraphDataModel::nBuses)
            column("Average system time (arriving)", GraphDataModel::avgSystemTimeArrivingPassenger)
            column("Confidence interval lower", GraphDataModel::lowSystemTimeArrivingPassenger)
            column("Confidence interval upper", GraphDataModel::uppSystemTimeArrivingPassenger)
            column("Average system time (leaving)", GraphDataModel::avgSystemTimeLeavingPassenger)
            column("Confidence interval lower", GraphDataModel::lowSystemTimeLeavingPassenger)
            column("Confidence interval upper", GraphDataModel::uppSystemTimeLeavingPassenger)
            column("Total costs", GraphDataModel::totalCosts)
            columnResizePolicy = TableView.CONSTRAINED_RESIZE_POLICY
            hgrow = Priority.ALWAYS
            vgrow = Priority.ALWAYS
        }
    }
}