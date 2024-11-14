const btn = document.querySelector('button');

btn.addEventListener('click', () => {
  external.sendMessage("A message from the frontend.")
});

external.receiveMessage((message) => {
  alert(message);
})