import { Link } from 'react-router-dom';

export function NotFoundPage() {
  return (
    <div className="panel">
      <h1>Not found</h1>
      <Link to="/app">Go back</Link>
    </div>
  );
}
