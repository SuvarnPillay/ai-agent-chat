import ChatUI from './ChatUI.jsx';

// ...existing code...
async function sendMessage(message, threadId) {
  const apiUrl = process.env.REACT_APP_API_URL || 'http://localhost:5000/api/chat/chat';
  const response = await fetch(
    `${apiUrl}`,
    {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ message, threadId }),
    }
  );
  if (!response.ok) throw new Error('Network response was not ok');
  const text = await response.text();
  return text;
}
// ...existing code...

function App() {
  return (
    <div className="App h-screen w-screen bg-gray-100">
      <ChatUI sendMessage={sendMessage} />
    </div>
  );
}

export default App;
