(function () {
  var todos = {
    init: function () {
      this.input = $('#todo-input');
      this.addBtn = $('#todo-add-btn');
      this.addBtn.on('click', this.onBtnClick.bind(this));
      this.list = $('#list');
      this.todo = $('.todo').first();
    },
    onBtnClick: function () {
      var task = this.input.val();
      this.input.val('');
      this.createTodo(task);
      this.postTodo(task);
    },
    loadTodos: function (onDone) {
      $.ajax({
        method: 'GET',
        url: 'http://localhost:3000/todos'
      }).done(onDone);
    },
    onLoadTodos: function (data) {
      for (var i = 0; i < data.length; i++) {
        console.log(data[i].task);
        this.createTodo(data[i].task);
      }
    },
    postTodo: function (todo) {
      console.log(todo);
      $.ajax({
        method: 'POST',
        url: 'http://localhost:3000/todo',
        contentType: 'application/json',
        data: JSON.stringify({
          task: todo
        })
      });
    },
    createTodo: function (text) {
      var todo = this.todo.clone();
      $(todo).find('.todo-text').text(text);
      $(todo).find('.todo-btn').on('click', this.removeTodo.bind(this));
      this.list.append(todo);
      todo.removeClass('invisible');
    },
    removeTodo: function (event) {
      var btn = event.target;
      var todo = $(btn).closest('.todo');
      todo.remove();
    },
    show: function () {
      this.init();
      this.loadTodos(this.onLoadTodos.bind(this));
    }
  };
  todos.show();
})();
