import { useEffect, useState } from "react";

const API_BASE = "https://finalproject-api-kf.azurewebsites.net/api/books";
const API_KEY = "YOUR_DEV_KEY_HERE"; // in real life, use env var

export default function App() {
  const [books, setBooks] = useState([]);
  const [title, setTitle] = useState("");
  const [author, setAuthor] = useState("");
  const [year, setYear] = useState("");
  const [message, setMessage] = useState("");

  async function fetchBooks() {
    const res = await fetch(API_BASE, {
      headers: { "x-api-key": API_KEY }
    });
    const data = await res.json();
    setBooks(data);
  }

  async function addBook(e) {
    e.preventDefault();
    const res = await fetch(API_BASE, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "x-api-key": API_KEY
      },
      body: JSON.stringify({ title, author, year: Number(year) })
    });
    if (res.ok) {
      setMessage("Book added");
      setTitle("");
      setAuthor("");
      setYear("");
      await fetchBooks();
    } else {
      setMessage("Error adding book");
    }
  }

  async function validateBooks() {
    const res = await fetch(`${API_BASE}/validate`, {
      method: "PATCH",
      headers: {
        "x-api-key": API_KEY
      }
    });
    const data = await res.json();
    setMessage(
      `Validation updated ${data.updatedCount} books at ${data.timestamp}`
    );
    await fetchBooks();
  }

  useEffect(() => {
    fetchBooks();
  }, []);

  return (
    <div style={{ maxWidth: "800px", margin: "0 auto", fontFamily: "system-ui" }}>
      <h1>Book Management Dashboard</h1>
      <p>{message}</p>

      <section>
        <h2>Add Book</h2>
        <form onSubmit={addBook}>
          <input
            placeholder="Title"
            value={title}
            onChange={e => setTitle(e.target.value)}
          />
          <input
            placeholder="Author"
            value={author}
            onChange={e => setAuthor(e.target.value)}
          />
          <input
            placeholder="Year"
            type="number"
            value={year}
            onChange={e => setYear(e.target.value)}
          />
          <button type="submit">Add</button>
        </form>
      </section>

      <section>
        <h2>Books</h2>
        <button onClick={validateBooks}>Run Validation</button>
        <ul>
          {books.map(b => (
            <li key={b.id}>
              {b.title} by {b.author} ({b.year}){" "}
              {b.archived ? "✅ Archived" : ""}
            </li>
          ))}
        </ul>
      </section>
    </div>
  );
}
