interface Props {
  message: string
  fieldErrors?: Record<string, string[]>
}

export function ErrorBanner({ message, fieldErrors }: Props) {
  return (
    <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
      <p className="font-medium">{message}</p>
      {fieldErrors && (
        <ul className="mt-1 list-inside list-disc">
          {Object.entries(fieldErrors).map(([campo, mensagens]) =>
            mensagens.map((msg) => <li key={`${campo}-${msg}`}>{msg}</li>),
          )}
        </ul>
      )}
    </div>
  )
}
