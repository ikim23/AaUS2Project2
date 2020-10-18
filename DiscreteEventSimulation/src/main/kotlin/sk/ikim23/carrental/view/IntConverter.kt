package sk.ikim23.carrental.view

import javafx.util.StringConverter
import tornadofx.*

class IntConverter : StringConverter<Number>() {
    override fun toString(n: Number?): String = (n ?: 0).toString()

    override fun fromString(s: String?): Number = if (s != null && s.isInt()) s.toInt() else 0
}