export async function copyToClipboard(text: string): Promise<boolean> {
  if (navigator.clipboard?.writeText /*&& window.isSecureContext*/) {
    try {
      await navigator.clipboard.writeText(text);
      return true;
    } catch (e) {
      console.error('Clipboard API copy failed, trying fallback', e);
      return fallbackCopy(text);
    }
  }
  return fallbackCopy(text);

  function fallbackCopy(text: string): boolean {
    let success = false, textarea = undefined;
    try {
      textarea = document.createElement('textarea');
      textarea.value = text ?? '';
      textarea.style.position = 'absolute';
      textarea.style.left = '-99999999px';
      document.body.prepend(textarea);
      textarea.select();
      success = document.execCommand('copy');
    } catch (e) {
      console.error('Fallback clipboardcopy failed', e);
    } finally {
      textarea?.remove();
    }
    return success;
  }
}