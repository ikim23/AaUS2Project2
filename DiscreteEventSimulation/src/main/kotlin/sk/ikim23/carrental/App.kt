package sk.ikim23.carrental

import javafx.stage.Stage
import sk.ikim23.carrental.core.Pauseable
import sk.ikim23.carrental.view.MainView
import tornadofx.App

class App : App(MainView::class) {
    override fun start(stage: Stage) {
        super.start(stage)
        stage.setOnCloseRequest {
            Pauseable.destroyAll()
        }
    }
}
