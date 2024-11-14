const sendUrl = () => {
  const url = document.getElementById('url-input').value;
  if (url) {
    window.external.sendMessage(url);
  } else {
    alert('Please enter a valid URL.');
  }
}

window.external.receiveMessage(message => {
  alert(message);
});