package sk.ikim23.montecarlo

import javafx.scene.Scene
import sk.ikim23.montecarlo.views.MainView
import tornadofx.*
import tornadofx.App

class App : App(MainView::class) {
    override fun createPrimaryScene(view: UIComponent) = Scene(view.root, 800.0, 600.0)
}
