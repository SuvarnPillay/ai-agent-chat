import ChatUI from './ChatUI.jsx';

// Minimal backend call to C# agent API (no useAgent flag)
async function sendMessage(message, threadId) {
  const response = await fetch('http://localhost:5000/api/chat/chat', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ message, threadId }),
  });
  if (!response.ok) throw new Error('Network response was not ok');
  const text = await response.text();
  return text;
}

function App() {
  return (
    <div className="App h-screen w-screen bg-gray-100">
      <ChatUI sendMessage={sendMessage} />
    </div>
  );
}

export default App;
