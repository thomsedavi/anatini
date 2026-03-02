<script setup lang="ts">

</script>

<template>
  <section id="panel-calendar" role="tabpanel" aria-labelledby="tab-calendar">
    <header>
      <h2 id="calendar-month">March 2026</h2>
      <nav aria-label="Calendar Navigation">
        <button type="button" aria-label="Previous Month">Previous</button>
        <button type="button" aria-label="Next Month">Next</button>
      </nav>
    </header>

    <table aria-labelledby="calendar-month">
      <thead>
        <tr>
          <th scope="col"><abbr title="Sunday">Sun</abbr></th>
          <th scope="col"><abbr title="Monday">Mon</abbr></th>
          <th scope="col"><abbr title="Tuesday">Tue</abbr></th>
          <th scope="col"><abbr title="Wednesday">Wed</abbr></th>
          <th scope="col"><abbr title="Thursday">Thu</abbr></th>
          <th scope="col"><abbr title="Friday">Fri</abbr></th>
          <th scope="col"><abbr title="Saturday">Sat</abbr></th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td></td>
          <td></td>
          <td></td>
          <td></td>
          <td></td>
          <td></td>
          <td>
            <time datetime="2026-03-01">1</time>
            <ul class="event-list">
              <li>
                <article>
                  <h3><a href="/events/123">Community Garden Meetup</a></h3>
                  <time datetime="2026-03-01T10:00">10:00 AM</time>
                </article>
              </li>
              <li>
                <article>
                  <h3><a href="/events/123">Community Garden Meetup</a></h3>
                  <time datetime="2026-03-01T10:00">10:00 AM</time>
                </article>
              </li>
              <li>
                <article>
                  <h3><a href="/events/123">Community Garden Meetup</a></h3>
                  <time datetime="2026-03-01T10:00">10:00 AM</time>
                </article>
              </li>
            </ul>
          </td>
        </tr>
        <tr>
          <td>
              <button type="button" aria-label="March 2, 2026">
                  <time datetime="2026-03-02">2</time>
              </button>
          </td>
        </tr>
      </tbody>
    </table>
  </section>
</template>
