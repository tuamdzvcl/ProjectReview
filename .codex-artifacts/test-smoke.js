const { chromium } = require("playwright");

const browserPath = "C:/Program Files/Google/Chrome/Application/chrome.exe";
const baseUrl = "http://localhost:4200";
const routes = [
  "/",
  "/login",
  "/profile/11111111-1111-1111-1111-111111111111",
];

async function testRoute(browser, route, viewport) {
  const page = await browser.newPage({ viewport });
  const consoleLogs = [];
  const failedResponses = [];

  page.on("console", (msg) => {
    if (["error", "warning", "warn"].includes(msg.type())) {
      consoleLogs.push({ type: msg.type(), text: msg.text() });
    }
  });

  page.on("pageerror", (err) => {
    consoleLogs.push({ type: "pageerror", text: err.message });
  });

  page.on("response", async (response) => {
    if (response.status() >= 400) {
      failedResponses.push({
        status: response.status(),
        url: response.url(),
      });
    }
  });

  await page.goto(`${baseUrl}${route}`, {
    waitUntil: "networkidle",
    timeout: 30000,
  });

  const fileSafeRoute = route === "/" ? "home" : route.replace(/[\\/:?\"<>|*]+/g, "_");
  await page.screenshot({
    path: `D:/Msf/.codex-artifacts/${viewport.width}x${viewport.height}-${fileSafeRoute}.png`,
    fullPage: true,
  });

  const title = await page.title();
  const bodyText = await page.locator("body").innerText();
  const h1Count = await page.locator("h1").count();
  const buttonCount = await page.getByRole("button", { name: "Đăng nhập" }).count().catch(() => 0);

  await page.close();

  return {
    route,
    viewport,
    title,
    bodySample: bodyText.slice(0, 1200),
    h1Count,
    loginButtonCount: buttonCount,
    consoleLogs,
    failedResponses,
  };
}

(async () => {
  const browser = await chromium.launch({
    headless: true,
    executablePath: browserPath,
  });

  const viewports = [
    { width: 1440, height: 900 },
    { width: 390, height: 844 },
  ];

  const results = [];

  for (const viewport of viewports) {
    for (const route of routes) {
      results.push(await testRoute(browser, route, viewport));
    }
  }

  await browser.close();
  console.log(JSON.stringify(results, null, 2));
})().catch((error) => {
  console.error(error);
  process.exit(1);
});
