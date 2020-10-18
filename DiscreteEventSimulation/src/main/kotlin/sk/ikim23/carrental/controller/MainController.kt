package sk.ikim23.carrental.controller

import sk.ikim23.carrental.core.impl.SimManager
import sk.ikim23.carrental.model.GraphModel
import sk.ikim23.carrental.model.InputModel
import sk.ikim23.carrental.model.ReplicationModel
import sk.ikim23.carrental.model.SummaryModel
import tornadofx.*

class MainController : Controller() {
    val inputModel = InputModel(10, 5, 10, 8, 12)
    val summaryModel = SummaryModel()
    val replicationModel = ReplicationModel()
    val graphModel = GraphModel()
    val manager = SimManager(replicationModel, summaryModel, graphModel)

    fun start() {
        replicationModel.clear()
        summaryModel.clear()
        graphModel.clear()
        manager.start(
                inputModel.busFrom.get(),
                inputModel.busTo.get(),
                inputModel.employeesFrom.get(),
                inputModel.employeesTo.get(),
                inputModel.nReplications.get()
        )
    }

    fun continueRun() {
        manager.unpause()
    }

    fun pause() {
        manager.pause()
    }

    fun stop() {
        manager.stop()
    }
}