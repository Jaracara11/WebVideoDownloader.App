document.addEventListener("DOMContentLoaded", () => {
  const downloadBtn = document.getElementById("download-btn");
  const urlInput = document.getElementById("url-input");
  const statusDiv = document.getElementById("status");

  downloadBtn.addEventListener("click", async () => {
    const videoUrl = urlInput.value.trim();

    if (!videoUrl) {
      statusDiv.textContent = "Please enter a video URL.";
      statusDiv.style.color = "red";
      return;
    }

    try {
      const response = await fetch("/api/download", {
        method: "POST",
        headers: { "Content-Type": "text/plain" },
        body: videoUrl,
      });

      if (response.ok) {
        const contentDisposition = response.headers.get('Content-Disposition');
        const filename = contentDisposition.match(/filename="(.+)"/)[1];

        const blob = await response.blob();
        const downloadUrl = window.URL.createObjectURL(blob);

        const a = document.createElement("a");
        a.href = downloadUrl;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        a.remove();

        window.URL.revokeObjectURL(downloadUrl);
        statusDiv.textContent = "Video downloaded successfully.";
        statusDiv.style.color = "green";
      } else {
        const error = await response.json();
        statusDiv.textContent = error.message || "Failed to download video.";
        statusDiv.style.color = "red";
      }
    } catch (error) {
      statusDiv.textContent = "Error: Unable to connect to the server.";
      statusDiv.style.color = "red";
    }
  });
});